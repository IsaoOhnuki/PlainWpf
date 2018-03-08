using SQLServerControls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseUtility
{
    public partial class Tables : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /// <summary>
            /// DefaultValue Setting
            /// </summary>
            modelBuilder.Conventions.Add(new DefaultValueAttributeConvention());

            base.OnModelCreating(modelBuilder);
        }
    }
    //https://blogs.yahoo.co.jp/dk521123/22779532.html 【C#】属性（アトリビュート）　～定義済み属性～
    //http://qiita.com/tomopy03/items/a8c30947f0753d89346a //Entitiy Framework Code Firstでデフォルト値の設定

    /// <summary>
    /// DefaultValue Setting
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValueAttribute : System.Attribute
    {
        public string DefaultValue { get; set; }
    }

    /// <summary>
    /// DefaultValue Setting
    /// </summary>
    public class DefaultValueAttributeConvention
        : PrimitivePropertyAttributeConfigurationConvention<DefaultValueAttribute>
    {
        public override void Apply(ConventionPrimitivePropertyConfiguration configuration, DefaultValueAttribute attribute)
        {
            configuration.HasColumnAnnotation("DefaultValue", attribute.DefaultValue);
        }
    }

    /// <summary>
    /// DefaultValue Setting
    /// </summary>
    internal class DefaultValueSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void Generate(AddColumnOperation addColumnOperation)
        {
            SetAnnotatedColumn(addColumnOperation.Column);
            base.Generate(addColumnOperation);
        }

        protected override void Generate(AlterColumnOperation alterColumnOperation)
        {
            SetAnnotatedColumn(alterColumnOperation.Column);
            base.Generate(alterColumnOperation);
        }

        protected override void Generate(CreateTableOperation createTableOperation)
        {
            SetAnnotatedColumns(createTableOperation.Columns);
            base.Generate(createTableOperation);
        }

        protected override void Generate(AlterTableOperation alterTableOperation)
        {
            SetAnnotatedColumns(alterTableOperation.Columns);
            base.Generate(alterTableOperation);
        }

        private void SetAnnotatedColumn(ColumnModel col)
        {
            AnnotationValues values;
            if (col.Annotations.TryGetValue("DefaultValue", out values))
            {
                if (values.NewValue != null)
                {
                    col.DefaultValueSql = (string)values.NewValue;
                }
                else
                {
                    col.DefaultValueSql = null;
                }
            }
        }

        private void SetAnnotatedColumns(IEnumerable<ColumnModel> columns)
        {
            foreach (var column in columns)
            {
                SetAnnotatedColumn(column);
            }
        }
    }
}
