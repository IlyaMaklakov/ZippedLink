using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MyCore.Web.Configuration;

namespace MyCore.AspNetCore.Mvc.Controllers
{
    public class AbpUserConfigurationController: AbpController
    {
        private readonly AbpUserConfigurationBuilder _abpUserConfigurationBuilder;

        public AbpUserConfigurationController(AbpUserConfigurationBuilder abpUserConfigurationBuilder)
        {
            this._abpUserConfigurationBuilder = abpUserConfigurationBuilder;
        }

        public async Task<JsonResult> GetAll()
        {
            var userConfig = await this._abpUserConfigurationBuilder.GetAll();
            return this.Json(userConfig);
        }
    }
}