//#define DATABASE_VERSION2

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DatabaseUtility
{
    // http://increment-i.hateblo.jp/archive/category/Entity%20Framework

    public partial class Tables : DbContext
    {
        // NuGet Console  "Add-Migration OrderNote"
        // NuGet Console  "Update-Database"

        //マイグレーションのタイムアウト
        //http://densan-labs.net/tech/codefirst/migration.html
        // NuGet Console  "Update-Database -Script"

        public static readonly string databaseName = "Tbl購買業務";

        public class DatabaseConnectionStateChangedParam
        {
            public DatabaseConnectionState DatabaseState { get; set; }
            public string ServerName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        private SqlConnectionStringBuilder connectionString;
        public event Action<DatabaseConnectionStateChangedParam> DatabaseConnectionStateChanged;
        private DatabaseConnectionState databaseState;
        public DatabaseConnectionState DatabaseState
        {
            get { return databaseState; }
            set
            {
                databaseState = value;
                connectionString = new SqlConnectionStringBuilder(Database.Connection.ConnectionString);
                if (databaseState == DatabaseConnectionState.DatabaseEnabled)
                {
                    Database.Connection.Open();
                }
                DatabaseConnectionStateChanged?.Invoke(new DatabaseConnectionStateChangedParam {
                    DatabaseState = databaseState,
                    ServerName = connectionString.DataSource,
                    UserName = connectionString.IntegratedSecurity ? null : connectionString.UserID,
                    Password = connectionString.IntegratedSecurity ? null : connectionString.Password
                });
            }
        }

        public Tables()
        : base(SQLServerConfigurationDialog.MakeConnectionString("", databaseName))
        {
        }

        public Tables(string server, Action<DatabaseConnectionStateChangedParam> databaseConnectionStateChanged = null)
            : base(SQLServerConfigurationDialog.MakeConnectionString(server, databaseName))
        {
            if (databaseConnectionStateChanged != null)
                DatabaseConnectionStateChanged += databaseConnectionStateChanged;
        }

        public Tables(string server, string user, string pass, Action<DatabaseConnectionStateChangedParam> databaseConnectionStateChanged = null)
            : base(SQLServerConfigurationDialog.MakeConnectionString(server, databaseName, user, pass))
        {
            if (databaseConnectionStateChanged != null)
                DatabaseConnectionStateChanged += databaseConnectionStateChanged;
        }

        public void TryConnection()
        {
            DatabaseState = SQLServerConfigurationDialog.TryConnection(Database.Connection.ConnectionString);
        }

        public DatabaseConnectionState? ShowDatabaseSelectDialog()
        {
            string conn = connectionString.ConnectionString;
            var ret = SQLServerConfigurationDialog.ShowDialog(ref conn, DialogInitializeType.ServerSelect, ServerConnectionAuthenticate.SQLServer);
            if (ret.HasValue)
            {
                if (ret.Value == DatabaseConnectionState.DatabaseEnabled)
                {
                    Database.Connection.Close();
                    Database.Connection.ConnectionString = conn;
                }
                DatabaseState = ret.Value;
            }
            return ret;
        }

        //http://blog.nakajix.jp/entry/2014/07/10/073000
        public override int SaveChanges()
        {
            var now = DateTime.Now;
            SetCreateDateTime(now);
            SetUpdateDateTime(now);
            return base.SaveChanges();
        }

        private void SetUpdateDateTime(DateTime now)
        {
            var entities = this.ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified) && e.CurrentValues.PropertyNames.Contains("UpdateDateTime"))
                .Select(e => e.Entity);

            foreach (dynamic entity in entities)
            {
                entity.UpdateDateTime = now;
            }
        }

        private void SetCreateDateTime(DateTime now)
        {
            var entities = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added && e.CurrentValues.PropertyNames.Contains("CreateDateTime"))
                .Select(e => e.Entity);

            foreach (dynamic entity in entities)
            {
                entity.CreateDateTime = now;
            }
        }

        private void SaveTimeFunction()
        {
            var entities = this.ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified))
                .Select(e => e.Entity);

            foreach (dynamic entity in entities)
            {
                entity.SaveTimeFunction();
            }
        }

        public static PropertyInfo[] GetKeyProperties(Type classType)
        {
            if (classType == null)
                return null;
            var keyProps = classType.GetProperties().Where(x => x.GetCustomAttribute(typeof(KeyAttribute)) != null);
            if (keyProps == null || keyProps.Count() == 0)
                return null;
            else
            {
                var idxProps = keyProps.Where(x => x.GetCustomAttribute(typeof(IndexAttribute)) != null);
                if (idxProps.Count() <= 1)
                    return keyProps.ToArray();
                else
                    return idxProps.OrderBy(x => (x.GetCustomAttribute(typeof(IndexAttribute)) as IndexAttribute).Order).ToArray();
            }
        }

        public static int GetStringMaxLength(Type classType, string propertyName)
        {
            //var info = classType.GetProperties().Where(x => x.PropertyType.IsPublic && x.PropertyType == typeof(string) && x.Name == propertyName).SingleOrDefault();
            var info = classType.GetProperty(propertyName, typeof(string));
            var attr = info?.GetCustomAttributes().Where(x => x.GetType() == typeof(MaxLengthAttribute)).SingleOrDefault() as MaxLengthAttribute;
            return attr == null ? 0 : attr.Length;
        }
    }


    public class ColorDataToColorConverter : IValueConverter
    {
        //データソース->表示データ
        public object Convert(object Value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ColorData value = Value as ColorData;
            if (value == null)
                value = System.Windows.Media.Colors.White;
            return System.Windows.Media.Color.FromArgb((byte)((value.color >> 24) & 0xff), (byte)((value.color >> 16) & 0xff), (byte)((value.color >> 8) & 0xff), (byte)(value.color & 0xff));
        }
        //表示データ->データソース
        public object ConvertBack(object Value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Media.Color value = Value == null ? System.Windows.Media.Colors.White : (System.Windows.Media.Color)Value;
            return new ColorData { color = (((Int32)value.A) << 24) + (((Int32)value.R) << 16) + (((Int32)value.G) << 8) + ((Int32)value.B) };
        }
    }

    public class ColorDataToBackColorConverter : IValueConverter
    {
        public static double ColorToValue(ColorData color)
        {
            Color col = color;
            return (
                ((double)col.R / 255) * 1.35
                +
                ((double)col.G / 255) * 2.65
                +
                ((double)col.B / 255)
                ) / 5.0;
        }

        public static Color BackColorToTextColor(Color col)
        {
            return ColorToValue(col) < 0.53 ? Colors.White : Colors.Black;
        }
        //データソース->表示データ
        public object Convert(object Value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ColorData value = Value as ColorData;
            if (value == null)
                value = System.Windows.Media.Colors.White;
            value = BackColorToTextColor(value);
            return System.Windows.Media.Color.FromArgb((byte)((value.color >> 24) & 0xff), (byte)((value.color >> 16) & 0xff), (byte)((value.color >> 8) & 0xff), (byte)(value.color & 0xff));
        }
        //表示データ->データソース
        public object ConvertBack(object Value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Media.Color value = Value == null ? System.Windows.Media.Colors.White : (System.Windows.Media.Color)Value;
            Color color = BackColorToTextColor(Color.FromArgb(value.A, value.R, value.G, value.B));
            return new ColorData { color = (((Int32)color.A) << 24) + (((Int32)color.R) << 16) + (((Int32)color.G) << 8) + ((Int32)color.B) };
        }
    }

    public class TableBase
    {
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (this.GetType() == obj?.GetType())
            {
                var propL = Tables.GetKeyProperties(this.GetType());
                var propR = Tables.GetKeyProperties(obj?.GetType());
                if (propL != null && propR != null && propL.Count() == propR.Count())
                {
                    bool ret = true;
                    for (int l = 0; ret && l < propL.Count(); ++l)
                        ret = propL[l].GetValue(this).Equals(propR[l].GetValue(obj));
                    return ret;
                }
            }
            return base.Equals(obj);
        }
        public virtual void SaveTimeFunction() { }
    }
}
