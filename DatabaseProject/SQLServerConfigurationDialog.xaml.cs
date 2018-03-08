using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
//using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DatabaseUtility
{
    public enum DialogInitializeType
    {
        // サーバー選択ダイアログ（データベース固定）
        ServerSelect,
        // データベース選択ダイアログ（サーバー固定）
        DatabaseSelect,
        // サーバー、データベース選択可能
        ServerAndDatabaseSelect,
    }

    public enum ServerConnectionAuthenticate
    {
        // 認証無し
        NoAuthenticate,
        // Windows認証デフォルト選択
        Windows,
        // SQLServer認証デフォルト選択
        SQLServer,
    }

    public enum DatabaseConnectionState
    {
        Disabled,
        ServerEnabled,
        DatabaseDisabled,
        DatabaseEnabled,
    }

    /// <summary>
    /// ServerDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SQLServerConfigurationDialog : Window
    {
        private SqlConnectionStringBuilder connectionStringBuilder;

        public static readonly DependencyProperty AuthenticateProperty = DependencyProperty.Register(
            nameof(Authenticate),
            typeof(ServerConnectionAuthenticate),
            typeof(SQLServerConfigurationDialog),
            new PropertyMetadata(default(ServerConnectionAuthenticate), (DependencyObject depObj, DependencyPropertyChangedEventArgs e) =>
            {
                SQLServerConfigurationDialog obj = depObj as SQLServerConfigurationDialog;
                obj.IsEnabledSQLServerAuthenticate = (ServerConnectionAuthenticate)e.NewValue == ServerConnectionAuthenticate.SQLServer;
                obj.IsEnableWindowsAuthenticate = (ServerConnectionAuthenticate)e.NewValue == ServerConnectionAuthenticate.Windows;
            }));

        public ServerConnectionAuthenticate Authenticate
        {
            get { return (ServerConnectionAuthenticate)GetValue(AuthenticateProperty); }
            set { SetValue(AuthenticateProperty, value); }
        }

        public string ServerName
        {
            get { return connectionStringBuilder.DataSource; }
            set
            {
                connectionStringBuilder.DataSource = value;
                AcceptButtonEnabled();
            }
        }

        public static readonly DependencyProperty IsShowServerNameProperty = DependencyProperty.Register(
            nameof(IsShowServerName),
            typeof(bool),
            typeof(SQLServerConfigurationDialog),
            new PropertyMetadata(defaultValue: true));

        public bool IsShowServerName
        {
            get { return (bool)GetValue(IsShowServerNameProperty); }
            set { SetValue(IsShowServerNameProperty, value); }
        }

        public string DatabaseName
        {
            get { return connectionStringBuilder.InitialCatalog; }
            set
            {
                connectionStringBuilder.InitialCatalog = value;
                AcceptButtonEnabled();
            }
        }

        public static readonly DependencyProperty IsShowDatabaseNameProperty = DependencyProperty.Register(
            nameof(IsShowDatabaseName),
            typeof(bool),
            typeof(SQLServerConfigurationDialog),
            new PropertyMetadata(true, (DependencyObject depObj, DependencyPropertyChangedEventArgs e) => {
                SQLServerConfigurationDialog obj = depObj as SQLServerConfigurationDialog;
                obj.AcceptButtonEnabled();
            }));

        public bool IsShowDatabaseName
        {
            get { return (bool)GetValue(IsShowDatabaseNameProperty); }
            set { SetValue(IsShowDatabaseNameProperty, value); }
        }

        public string UserName
        {
            get { return connectionStringBuilder.UserID; }
            set
            {
                connectionStringBuilder.UserID = value;
                AcceptButtonEnabled();
            }
        }

        public string Password
        {
            get { return connectionStringBuilder.Password; }
            set
            {
                connectionStringBuilder.Password = value;
                if (passWord.Password != value)
                    passWord.Password = value;
            }
        }

        public static readonly DependencyProperty IsShowAuthenticateProperty = DependencyProperty.Register(
            nameof(IsShowAuthenticate),
            typeof(bool),
            typeof(SQLServerConfigurationDialog),
            new PropertyMetadata(true, (DependencyObject depObj, DependencyPropertyChangedEventArgs e) => {
                SQLServerConfigurationDialog obj = depObj as SQLServerConfigurationDialog;
                obj.AcceptButtonEnabled();
            }));

        public bool IsShowAuthenticate
        {
            get { return (bool)GetValue(IsShowAuthenticateProperty); }
            set { SetValue(IsShowAuthenticateProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledSQLServerAuthenticateProperty = DependencyProperty.Register(
            nameof(IsEnabledSQLServerAuthenticate),
            typeof(bool),
            typeof(SQLServerConfigurationDialog),
            new PropertyMetadata(false, propertyChangedCallback: (DependencyObject depObj, DependencyPropertyChangedEventArgs e) => {
                SQLServerConfigurationDialog obj = depObj as SQLServerConfigurationDialog;
                obj.connectionStringBuilder.IntegratedSecurity = !(bool)e.NewValue;
                obj.AcceptButtonEnabled();
            }));

        public bool IsEnabledSQLServerAuthenticate
        {
            get { return (bool)GetValue(IsEnabledSQLServerAuthenticateProperty); }
            set { SetValue(IsEnabledSQLServerAuthenticateProperty, value); }
        }

        public static readonly DependencyProperty IsEnableWindowsAuthenticateProperty = DependencyProperty.Register(
            nameof(IsEnableWindowsAuthenticate),
            typeof(bool),
            typeof(SQLServerConfigurationDialog),
            new PropertyMetadata(false, propertyChangedCallback: (DependencyObject depObj, DependencyPropertyChangedEventArgs e) => {
                SQLServerConfigurationDialog obj = depObj as SQLServerConfigurationDialog;
                obj.connectionStringBuilder.IntegratedSecurity = (bool)e.NewValue;
                obj.AcceptButtonEnabled();
            }));

        public bool IsEnableWindowsAuthenticate
        {
            get { return (bool)GetValue(IsEnableWindowsAuthenticateProperty); }
            set { SetValue(IsEnableWindowsAuthenticateProperty, value); }
        }

        public ObservableCollection<string> Databases { get; set; }

        public SQLServerConfigurationDialog(string connectionString)
        {
            InitializeComponent();

            connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            passWord.Password = Password;

            InitializeDatabases();

            DataContext = this;

            AcceptButtonEnabled();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = ((PasswordBox)sender).Password;
            AcceptButtonEnabled();
        }

        private void InitializeDatabases(string[] list = null)
        {
            if (Databases == null)
                Databases = new ObservableCollection<string>();
            else
                Databases.Clear();
            Databases.Add("<最新の情報に更新>");
            if (list != null)
                Databases.AddRange(list);
        }

        public string SelectedDatabase
        {
            get { return ""; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && Databases[0] == value)
                {
                    InitializeDatabases(EnumDatabase());
                }
            }
        }

        public static string[] EnumDatabase(SqlConnectionStringBuilder connectionStringBuilder, string databaseNameFilter = null)
        {
            System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString);
            string[] ret;
            try
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT NAME, DATABASE_ID, CREATE_DATE FROM SYS.DATABASES WHERE NAME NOT IN ('master','tempdb','model','msdb');";
                System.Data.SqlClient.SqlDataAdapter adapter = new SqlDataAdapter(command);
                System.Data.DataTable table = new DataTable();
                adapter.Fill(table);
                if (string.IsNullOrWhiteSpace(databaseNameFilter))
                    ret = table.AsEnumerable().Select(x => x.Field<string>("NAME")).ToArray();
                else
                {
                    try
                    {
                        ret = table.AsEnumerable().Select(x => x.Field<string>("NAME")).Where(x => Regex.IsMatch(x, databaseNameFilter)).ToArray();
                    }
                    catch
                    {
                        ret = table.AsEnumerable().Select(x => x.Field<string>("NAME")).ToArray();
                    }
                }
                connection.Close();
            }
            catch
            {
                ret = new string[0];
            }
            return ret;
        }

        public static string[] EnumDatabase(string connectionString, string databaseNameFilter = null)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            return EnumDatabase(connectionStringBuilder, databaseNameFilter);
        }

        public string[] EnumDatabase()
        {
            return EnumDatabase(connectionStringBuilder, DatabaseNameFilter);
        }

        private void AcceptButtonEnabled()
        {
            AcceptButton.IsEnabled = (!IsShowServerName || !string.IsNullOrWhiteSpace(ServerName))
                && (!IsShowAuthenticate || IsEnableWindowsAuthenticate || IsEnabledSQLServerAuthenticate && !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
                && (!IsShowDatabaseName || !string.IsNullOrWhiteSpace(DatabaseName));
        }

        DatabaseConnectionState ResultState { get; set; }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var cursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
            this.IsEnabled = false;
            ResultState = await TryConnectionAsync(connectionStringBuilder);
            this.IsEnabled = true;
            Mouse.OverrideCursor = cursor;
            if (ResultState == DatabaseConnectionState.DatabaseEnabled || ResultState == DatabaseConnectionState.ServerEnabled && WeakConfiguration)
                DialogResult = true;
            else
                MessageBox.Show("データベースサーバーに接続できませんでした", "データベースサーバー接続エラー", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = !IsEnabled;
        }

        public bool WeakConfiguration { get; set; }

        public string DatabaseNameFilter { get; set; }

        public static DatabaseConnectionState? ShowDialog(ref string connectionString, DialogInitializeType initializeType, ServerConnectionAuthenticate authenticate
            , bool weakConfiguration = false, string databaseNameFilter = null)
        {
            SQLServerConfigurationDialog dlg = new SQLServerConfigurationDialog(connectionString);
            dlg.WeakConfiguration = weakConfiguration;
            dlg.DatabaseNameFilter = databaseNameFilter;
            dlg.Authenticate = authenticate;
            dlg.IsShowAuthenticate = authenticate != ServerConnectionAuthenticate.NoAuthenticate;
            dlg.IsShowServerName = initializeType != DialogInitializeType.DatabaseSelect;
            dlg.IsShowDatabaseName = initializeType != DialogInitializeType.ServerSelect;
            bool? ret = dlg.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                connectionString = dlg.connectionStringBuilder.ConnectionString;
                return dlg.ResultState;
            }
            else
                return null;
        }

        public static Task<DatabaseConnectionState> TryConnectionAsync(string connectionString)
        {
            return TryConnectionAsync(new SqlConnectionStringBuilder(connectionString));
        }

        public static Task<DatabaseConnectionState> TryConnectionAsync(SqlConnectionStringBuilder connectionStringBuilder)
        {
            return Task<DatabaseConnectionState>.Run(() => TryConnection(connectionStringBuilder));
        }

        public static DatabaseConnectionState TryConnection(string connectionString)
        {
            return TryConnection(new SqlConnectionStringBuilder(connectionString));
        }

        public static DatabaseConnectionState TryConnection(SqlConnectionStringBuilder connectionStringBuilder)
        {
            string database = connectionStringBuilder.InitialCatalog;
            connectionStringBuilder.InitialCatalog = "";
            DbConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString);
            connectionStringBuilder.InitialCatalog = database;
            try
            {
                connection.Open();
                connection.Close();
                try
                {
                    connection = new SqlConnection(connectionStringBuilder.ConnectionString);
                    connection.Open();
                    connection.Close();
                    return DatabaseConnectionState.DatabaseEnabled;
                }
                catch
                {
                    var db = EnumDatabase(connectionStringBuilder);
                    if (db.Any(x => x == database))
                        return DatabaseConnectionState.DatabaseDisabled;
                    else
                        return DatabaseConnectionState.ServerEnabled;
                }
            }
            catch
            {
                return DatabaseConnectionState.Disabled;
            }
        }

        public static DatabaseConnectionState TryConnection(Database database)
        {
            return TryConnection(database.Connection.ConnectionString);
        }

        public static Task<DatabaseConnectionState> TryConnectionAsync(Database database)
        {
            return TryConnectionAsync(database.Connection.ConnectionString);
        }

        public static DatabaseConnectionState? ShowDialog(Database database, DialogInitializeType type, ServerConnectionAuthenticate authenticate = ServerConnectionAuthenticate.NoAuthenticate
            , bool weakConfiguration = false, string databaseNameFilter = null, Action<DatabaseConnectionState> databaseConnectionStateChanged = null)
        {
            string connectionString = database.Connection.ConnectionString;
            DatabaseConnectionState? ret = ShowDialog(ref connectionString, type, authenticate, weakConfiguration, databaseNameFilter);
            if (ret.HasValue)
            {
                SqlConnectionStringBuilder oldConnectionString = new SqlConnectionStringBuilder(database.Connection.ConnectionString);
                SqlConnectionStringBuilder newConnectionString = new SqlConnectionStringBuilder(connectionString);
                //if (newConnectionString.DataSource != oldConnectionString.DataSource || newConnectionString.InitialCatalog != oldConnectionString.InitialCatalog)
                databaseConnectionStateChanged?.Invoke(DatabaseConnectionState.Disabled);
                database.Connection.ConnectionString = connectionString;
                databaseConnectionStateChanged?.Invoke(ret.Value);
            }
            return ret;
        }

        public static string MakeConnectionString(string server, string database)
        {
            return "Data Source=" + server + ";Initial Catalog=" + database + ";Integrated Security=True;Persist Security Info=False;multipleactiveresultsets=True;";
        }

        public static string MakeConnectionString(string server, string database, string user, string password)
        {
            return "Data Source=" + server + ";Initial Catalog=" + database + ";User ID=" + user + ";Password=" + password + ";Persist Security Info=False;multipleactiveresultsets=True;";
        }
    }
}
