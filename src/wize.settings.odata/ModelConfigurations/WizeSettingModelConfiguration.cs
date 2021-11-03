using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wize.settings.data.V1.Models;

namespace wize.settings.odata.ModelConfigurations
{
    public class WizeSettingModelConfiguration : IModelConfiguration
    {
        public void Apply(ODataModelBuilder builder, ApiVersion version, string routePrefix)
        {
            switch(version.MajorVersion)
            {
                case 1:
                    BuildV1(builder);
                    break;
                default:
                    BuildDefault(builder);
                    break;
            }
        }

        private EntityTypeConfiguration<WizeSetting> BuildDefault(ODataModelBuilder builder)
        {
            builder.EntityType<WizeSetting>().HasKey(m => new { m.Type, m.Name });
            builder.EntitySet<WizeSetting>("WizeSettings");
            builder.Action("Update").Returns<IActionResult>().Parameter<List<WizeSetting>>("model");
            return builder.EntitySet<WizeSetting>("WizeSettings").EntityType;
        }

        private void BuildV1(ODataModelBuilder builder)
        {
            BuildDefault(builder);//.Ignore(something);
        }
    }
}
