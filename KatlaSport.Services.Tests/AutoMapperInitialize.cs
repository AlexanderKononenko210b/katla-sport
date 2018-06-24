using System;
using System.Threading;
using AutoMapper;
using KatlaSport.Services.HiveManagement;

namespace KatlaSport.Services.Tests
{
    /// <summary>
    /// Class type singlton for initialize AutoMapper
    /// </summary>
    public sealed class AutoMapperInitialize
    {
        private static readonly Lazy<AutoMapperInitialize> instance =
            new Lazy<AutoMapperInitialize>(() => new AutoMapperInitialize(), LazyThreadSafetyMode.ExecutionAndPublication);

        private AutoMapperInitialize()
        {
            Mapper.Reset();
            Mapper.Initialize(x =>
            {
                x.AddProfile<HiveManagementMappingProfile>();
            });
        }

        public static AutoMapperInitialize Instance => instance.Value;
    }
}
