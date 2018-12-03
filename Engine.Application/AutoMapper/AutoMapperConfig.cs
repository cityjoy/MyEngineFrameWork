using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Application.DTO;
using AutoMapper;
using Engine.Domain.Entity;
namespace Engine.Application
{
    public static class AutoMapperConfig
    {
        /// <summary>
        /// 注册DTO映射
        /// </summary>
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Student, StudentDto>();

            });
        }
    }
}
