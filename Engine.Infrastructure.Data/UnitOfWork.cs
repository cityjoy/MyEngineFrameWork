using Engine.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Data
{
    /// <summary>
    /// 工作单元实现类
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region 数据上下文

        /// <summary>
        /// 数据上下文
        /// </summary>
        private IEFDbContext dbContext;
        public UnitOfWork(IEFDbContext context)
        {
            this.dbContext = context;
        }

        #endregion
        public bool Commit()
        {
            bool commited = false;
            try
            {
                commited = dbContext.SaveChanges() > 0;

            }
            catch (DbEntityValidationException ex)
            {

                var msg = string.Empty;//记录错误信息
                foreach (var errorList in ex.EntityValidationErrors)
                {
                    foreach (var item in errorList.ValidationErrors)
                    {
                        msg += string.Format("Property:{0},Error:{1}", item.PropertyName, item.ErrorMessage) + Environment.NewLine;
                    }

                }
                LogHelper.WriteLog("DbEntityValidationException(校验数据异常)" + msg);

            }
            catch (DbUpdateException ex)
            {
                LogHelper.WriteLog("DbUpdateException(提交数据异常)" + ex.InnerException.ToString());
            }

            return commited;
        }

        public void Dispose()
        {
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
