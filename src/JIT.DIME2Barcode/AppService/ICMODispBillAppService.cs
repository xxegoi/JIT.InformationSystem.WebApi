﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using JIT.DIME2Barcode.Entities;
using JIT.DIME2Barcode.TaskAssignment.ICMODispBill.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JIT.DIME2Barcode.AppService
{
    /// <summary>
    /// 派工单接口服务
    /// </summary>
    public class ICMODispBillAppService:ApplicationService
    {
        public IRepository<VW_ICMODispBill_By_Date,string> VRepository { get; set; }
        public IRepository<ICMODaily,string> DRepository { get; set; }
        public IRepository<ICMODispBill,string> Repository { get; set; }

        /// <summary>
        /// 任务派工界面数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public  async  Task<PagedResultDto<ICMODispBillListDto>> GetAll(ICMODispBillGetAllInput input)
        {
            var query = VRepository.GetAll().Where(p=>p.日期==input.FDate);

            var count = await query.CountAsync();
            try
            {
                var data = query.ToList();
                var list = data.MapTo(new List<ICMODispBillListDto>());

                return new PagedResultDto<ICMODispBillListDto>(count, list);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
        /// <summary>
        /// 日计划单生成或更新派工单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public  async Task<ICMODispBillListDto> Create(ICMODispBillCreateInput input)
        {
            decimal? totalCommitQty = 0;


            foreach (var dispBillI in input.Details)
            {
                var dailyFid = dispBillI.FSrcID;
                var entity =await Repository.GetAll().SingleOrDefaultAsync(p => p.FSrcID == dailyFid);
                if (entity == null)
                {
                    /*
                     *派工单不存在，插入新派工单
                     */
                    entity = new ICMODispBill()
                    {
                        FSrcID = dailyFid,
                        FWorkCenterID = dispBillI.FWorkCenterID,
                        FMachineID = dispBillI.FMachineID,
                        FMOBillNo = dispBillI.FMOBillNo,
                        FMOInterID =dispBillI.FMOInterID,
                        FCommitAuxQty = dispBillI.FCommitAuxQty,
                        FBiller = AbpSession.UserId.ToString(),
                        FDate = DateTime.Now
                    };

                    totalCommitQty += dispBillI.FCommitAuxQty;

                    await  Repository.InsertAsync(entity);

                }
                else
                {
                    /*
                     *派工单已存在，更新派工单信息
                     */
                    entity.FWorkCenterID = dispBillI.FWorkCenterID;
                    entity.FWorker = dispBillI.FWorker;
                    entity.FCommitAuxQty = dispBillI.FCommitAuxQty;
                    entity.FMachineID = dispBillI.FMachineID;
                    entity.FDate = DateTime.Now;

                    await Repository.UpdateAsync(entity);

                    totalCommitQty += entity.FCommitAuxQty;
                }
            }

            return null;

        }


    }
}