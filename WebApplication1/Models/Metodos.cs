using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using static WebApplication1.Controllers.HomeController;
using System.Data.Common;
using System.Collections;

namespace WebApplication1.Models
{
    public class Metodos 
    {
        public static DataTable mandaQry(string sqry,ref string Result)
        {
            //10.77.112.3 // 34.94.136.180
            //var cadenaConexion = "sslmode = None; server = 34.94.136.180; user id = root; password = PDN_2021; database = PDNCOAH; pooling = True; maxpoolsize = 5; minpoolsize = 0; connectiontimeout = 15; connectionlifetime = 1800";
            //var cadenaConexion = "sslmode=None;server=/cloudsql/clever-overview-326002:us-west2:pdncoah;user id=root;password=PDN_2021;database=PDNCOAH;protocol=Unix;connectiontimeout=15;defaultcommandtimeout=5000;pooling=True;maxpoolsize=5;minpoolsize=0;connectionlifetime=1800";
            //var cadenaConexion = "server=10.77.112.3; database=PDNCOAH; Uid=root; pwd=PDN_2021;";
            var cadenaConexion = "server=143.110.237.45; database=PDNCOAH; Uid=sistemas; pwd=Leon4rdo..;";
            MySqlConnection conexion = new MySqlConnection(cadenaConexion);
            MySqlCommand comando;
            MySqlDataAdapter adaptador;
            DataTable tbl = new DataTable();

            Result = "";

            try
            {

                if (!string.IsNullOrWhiteSpace(cadenaConexion))
                {

                    comando = new MySqlCommand(sqry, conexion);
                    comando.CommandType = CommandType.Text;
                    tbl = new DataTable();
                    adaptador = new MySqlDataAdapter(comando);
                    adaptador.Fill(tbl);

                }
            }
            catch(Exception ex)
            {
                Result = ex.Message;

            }
            return tbl;
        }


        public static string GetSHA256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
        private readonly MySqlConnectionStringBuilder _connectionString;
        public static MySqlConnectionStringBuilder GetMySqlConnectionString()
        {
            MySqlConnectionStringBuilder connectionString;
            if (Environment.GetEnvironmentVariable("DB_HOST") != null)
            {
                connectionString = NewMysqlTCPConnectionString();
               
            }
            else
            {
                connectionString = NewMysqlUnixSocketConnectionString();
            }
            // The values set here are for demonstration purposes only. You 
            // should set these values to what works best for your application.
            // [START cloud_sql_mysql_dotnet_ado_limit]
            // MaximumPoolSize sets maximum number of connections allowed in the pool.
            connectionString.MaximumPoolSize = 5;
            // MinimumPoolSize sets the minimum number of connections in the pool.
            connectionString.MinimumPoolSize = 0;
            // [END cloud_sql_mysql_dotnet_ado_limit]
            // [START cloud_sql_mysql_dotnet_ado_timeout]
            // ConnectionTimeout sets the time to wait (in seconds) while
            // trying to establish a connection before terminating the attempt.
            connectionString.ConnectionTimeout = 15;
            // [END cloud_sql_mysql_dotnet_ado_timeout]
            // [START cloud_sql_mysql_dotnet_ado_lifetime]
            // ConnectionLifeTime sets the lifetime of a pooled connection
            // (in seconds) that a connection lives before it is destroyed
            // and recreated. Connections that are returned to the pool are
            // destroyed if it's been more than the number of seconds
            // specified by ConnectionLifeTime since the connection was
            // created. The default value is zero (0) which means the
            // connection always returns to pool.
            connectionString.ConnectionLifeTime = 1800; // 30 minutes
            // [END cloud_sql_mysql_dotnet_ado_lifetime]
            return connectionString;
        }

        public static MySqlConnectionStringBuilder NewMysqlTCPConnectionString()
        {
            // [START cloud_sql_mysql_dotnet_ado_connection_tcp]
            // Equivalent connection string:
            // "Uid=<DB_USER>;Pwd=<DB_PASS>;Host=<DB_HOST>;Database=<DB_NAME>;"
            var connectionString = new MySqlConnectionStringBuilder()
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance.
                SslMode = MySqlSslMode.None,

                // Remember - storing secrets in plain text is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                Server = Environment.GetEnvironmentVariable("DB_HOST"),   // e.g. '127.0.0.1'
                // Set Host to 'cloudsql' when deploying to App Engine Flexible environment
                UserID = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user'
                Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
            };
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;
            // [END cloud_sql_mysql_dotnet_ado_connection_tcp]
        }

        public static MySqlConnectionStringBuilder NewMysqlUnixSocketConnectionString()
        {
            // [START cloud_sql_mysql_dotnet_ado_connection_socket]
            // Equivalent connection string:
            // "Server=<dbSocketDir>/<INSTANCE_CONNECTION_NAME>;Uid=<DB_USER>;Pwd=<DB_PASS>;Database=<DB_NAME>;Protocol=unix"
            String dbSocketDir = Environment.GetEnvironmentVariable("DB_SOCKET_DIR") ?? "/cloudsql";
            String instanceConnectionName = "clever-overview-326002:us-west2:pdncoah";
            var connectionString = new MySqlConnectionStringBuilder()
            {
                // The Cloud SQL proxy provides encryption between the proxy and instance.
                SslMode = MySqlSslMode.None,
                // Remember - storing secrets in plain text is potentially unsafe. Consider using
                // something like https://cloud.google.com/secret-manager/docs/overview to help keep
                // secrets secret.
                Server = String.Format("{0}/{1}", dbSocketDir, instanceConnectionName),
                UserID = "root",   // e.g. 'my-db-user
                Password = "PDN_2021", // e.g. 'my-db-password'
                Database = "PDNCOAH", // e.g. 'my-database'
                ConnectionProtocol = MySqlConnectionProtocol.UnixSocket,    
                ConnectionTimeout = 300,
                DefaultCommandTimeout = 5000
            };
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;
            // [END cloud_sql_mysql_dotnet_ado_connection_socket]
        }
        public static List<Object> CargaDependecias()
        {

            var connectionString = GetMySqlConnectionString();
            DataTable tbl = new DataTable();
            using (DbConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                //connection.OpenWithRetry();
                using (var createTableCommand = connection.CreateCommand())
                {
                    createTableCommand.CommandText = @"
                       SELECT IdDependencia, SiglasDependencia, Dependencia FROM PDNCOAH.Dependencia";

                    //var reader = createTableCommand.ExecuteReader();
                    //tbl.Load(reader);
                    string sqry = string.Empty;
                    string errCode = "";
                    sqry = "SELECT IdDependencia, SiglasDependencia, Dependencia FROM PDNCOAH.Dependencia";
                    
                    tbl = mandaQry(sqry, ref errCode);
                    Dependencias depen = new Dependencias();
                    List<Object> ListaDepend = new List<Object>();

                        for (int i = 0; i < tbl.Rows.Count; i++)
                        {
                            depen = new Dependencias();
                            depen.nombre = tbl.Rows[i][2].ToString();
                            depen.siglas = tbl.Rows[i][1].ToString();
                            depen.clave = tbl.Rows[i][0].ToString();
                            ListaDepend.Add(depen);
                        }

                    return ListaDepend;
                }
            }

            
           

        }

        public static DataTable GetServidor(string NombreTabla, DateTime FechaIni, DateTime FechaFin, string Dependencia, string Puesto)
        {
            DataTable Result = null;
            string errCode="";

            try
            {
               
                    string Query = string.Empty;

                    switch (NombreTabla)
                    {
                        case "Servidores":
                            Query = @"SELECT DISTINCT NOMBRE FROM V_Servidores WHERE FechaCaptura BETWEEN '" + FechaIni.ToString("yyyy-MM-dd") + "' AND '" + FechaFin.ToString("yyyy-MM-dd") + "'";
                            break;
                        default:
                            Query = string.Empty;
                            break;
                    }
                    if (Dependencia != null && Dependencia != "null")
                    {
                        if (Dependencia != string.Empty)
                        {
                            Query = Query + " AND Dependencia = '" + Dependencia + "'";
                        }
                    }
                    if (Puesto != null && Puesto != "null")
                    {
                        if (Puesto != string.Empty)
                        {
                            Query = Query + " AND Puesto = '" + Puesto + "'";
                        }
                    }
                    if (Query != string.Empty)
                    {
                        Result = mandaQry(Query,ref errCode);
                    }
                
            }
            catch (Exception ex)
            {
                errCode = ex.Message;
               
            }

            return Result;
        }
       
        public static DataTable GetDependencia(string NombreTabla, DateTime FechaIni, DateTime FechaFin)
        {
            DataTable Result = null;

           
                    string Query = string.Empty;
            string errCode = "";
            switch (NombreTabla)
                    {
                        case "Servidores":
                            Query = @"SELECT DISTINCT DEPENDENCIA FROM V_Servidores WHERE FechaCaptura BETWEEN '" + FechaIni.ToString("yyyy-MM-dd") + "' AND '" + FechaFin.ToString("yyyy-MM-dd") + "'";
                            break;
                        default:
                            Query = string.Empty;
                            break;
                    }

                    if (Query != string.Empty)
                    {

                        Result = mandaQry(Query,ref errCode);
                    }
           

            return Result;
        }
        
        public static DataTable GetPuesto(string NombreTabla, DateTime FechaIni, DateTime FechaFin, string Dependencia)
        {
            DataTable Result = null;
            string errCode = "";
          
                    string Query = string.Empty;

                    switch (NombreTabla)
                    {
                        case "Servidores":
                            Query = @"SELECT DISTINCT PUESTO FROM V_Servidores WHERE FechaCaptura BETWEEN '" + FechaIni.ToString("yyyy-MM-dd") + "' AND '" + FechaFin.ToString("yyyy-MM-dd") + "'";
                            break;
                        default:
                            Query = string.Empty;
                            break;
                    }
                    if (Dependencia != null && Dependencia != "null")
                    {
                        if (Dependencia != string.Empty)
                        {
                            Query = Query + " AND Dependencia = '" + Dependencia + "'";
                        }
                    }

                    if (Query != string.Empty)
                    {

                        Result =mandaQry(Query,ref errCode);
                    }
             

            return Result;
        }

        
        public static DataTable GetTabla(string NombreTabla, DateTime FechaIni, DateTime FechaFin, string Dependencia, string Puesto, string Servidor)
        {
            DataTable Result = null;
            string errCode = "";
          
                    string Query = string.Empty;

                    switch (NombreTabla)
                    {
                        case "Servidores":
                            Query = @"SELECT IdServidor, FechaCaptura as Fecha, EjercicioFiscal as Ejercicio, Ramo, Dependencia,  Nombre, Puesto FROM V_Servidores WHERE FechaCaptura BETWEEN '" + FechaIni.ToString("yyyy-MM-dd") + "' AND '" + FechaFin.ToString("yyyy-MM-dd") + "'";
                            break;
                        default:
                            Query = string.Empty;
                            break;
                    }
                    if (Servidor != null && Servidor != "null")
                    {
                        if (Servidor != string.Empty)
                        {
                            Query = Query + " AND Nombre = '" + Servidor + "'";
                        }
                    }
                    if (Dependencia != null && Dependencia != "null")
                    {
                        if (Dependencia != string.Empty)
                        {
                            Query = Query + " AND Dependencia = '" + Dependencia + "'";
                        }
                    }
                    if (Puesto != null && Puesto != "null")
                    {
                        if (Puesto != string.Empty)
                        {
                            Query = Query + " AND Puesto = '" + Puesto + "'";
                        }
                    }
                    if (Query != string.Empty)
                    {
                        Result =mandaQry(Query,ref errCode);
                    }

            return Result;
        }


        public static DataTable spic(reqSpic reqSpic)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;
           // Query = @"Select distinct ServidoresEnContrataciones.IdServidorEnContrataciones, FechaCaptura, EjercicioFiscal, ServidoresEnContrataciones.IdRamo, Ramo, Servidores.RFC, Servidores.CURP, Servidores.Nombres, Servidores.PrimerApellido, Servidores.SegundoApellido,  Servidores.IdGenero, Genero, Dependencia, SiglasDependencia, ServidoresEnContrataciones.IdDependencia, Puesto.Puesto, Servidores.IdPuesto, SuperiorInmediato.Nombres as NombresSup, SuperiorInmediato.PrimerApellido as PrimerApellidoSup, SuperiorInmediato.SegundoApellido as SegundoApellidoSup, SuperiorInmediato.CURP as CURPSup, SuperiorInmediato.RFC as RFCSup, PuestoSup.Puesto as PuestoSup, SuperiorInmediato.IdPuesto as IdPuestoSup from ServidoresEnContrataciones inner join Servidores on ServidoresEnContrataciones.IdServidor = Servidores.IdServidor inner join Ramo on Ramo.IdRamo = ServidoresEnContrataciones.IdRamo inner join Genero on Genero.idgenero = Servidores.IdGenero inner join Dependencia on Dependencia.IdDependencia = ServidoresEnContrataciones.IdDependencia inner join Puesto on Puesto.IdPuesto = Servidores.IdPuesto inner join Servidores as SuperiorInmediato on SuperiorInmediato.IdServidor= ServidoresEnContrataciones.IdSuperiorInmediato inner join Puesto PuestoSup on PuestoSup.IdPuesto = SuperiorInmediato.IdPuesto inner join ProcedimientoServidor on ProcedimientoServidor.IdServidorEnContrataciones = ServidoresEnContrataciones.IdServidorEnContrataciones where ServidoresEnContrataciones.IdServidorEnContrataciones is not null ";
            Query = @"Select distinct ServidoresEnContrataciones.IdServidorEnContrataciones, FechaCaptura, EjercicioFiscal, ServidoresEnContrataciones.IdRamo, Ramo, NULL AS RFC, NULL AS CURP, Servidores.Nombres, Servidores.PrimerApellido, Servidores.SegundoApellido,  Servidores.IdGenero, Genero, Dependencia, SiglasDependencia, ServidoresEnContrataciones.IdDependencia, Puesto.Puesto, Servidores.IdPuesto, SuperiorInmediato.Nombres as NombresSup, SuperiorInmediato.PrimerApellido as PrimerApellidoSup, SuperiorInmediato.SegundoApellido as SegundoApellidoSup, NULL as CURPSup, NULL as RFCSup, PuestoSup.Puesto as PuestoSup, SuperiorInmediato.IdPuesto as IdPuestoSup from ServidoresEnContrataciones inner join Servidores on ServidoresEnContrataciones.IdServidor = Servidores.IdServidor inner join Ramo on Ramo.IdRamo = ServidoresEnContrataciones.IdRamo inner join Genero on Genero.idgenero = Servidores.IdGenero inner join Dependencia on Dependencia.IdDependencia = ServidoresEnContrataciones.IdDependencia inner join Puesto on Puesto.IdPuesto = Servidores.IdPuesto inner join Servidores as SuperiorInmediato on SuperiorInmediato.IdServidor= ServidoresEnContrataciones.IdSuperiorInmediato inner join Puesto PuestoSup on PuestoSup.IdPuesto = SuperiorInmediato.IdPuesto inner join ProcedimientoServidor on ProcedimientoServidor.IdServidorEnContrataciones = ServidoresEnContrataciones.IdServidorEnContrataciones where ServidoresEnContrataciones.IdServidorEnContrataciones is not null ";

            if (reqSpic.query != null)
            {
                if (reqSpic.query.id != null)
                {
                    if (reqSpic.query.id != string.Empty)
                    {
                        Query = Query + " AND ServidoresEnContrataciones.IdServidorEnContrataciones LIKE '%" + reqSpic.query.id + "%'";
                    }
                }
                if (reqSpic.query.nombres != null)
                {
                    if (reqSpic.query.nombres != string.Empty)
                    {
                        Query = Query + " AND Servidores.Nombres LIKE '%" + reqSpic.query.nombres + "%'";
                    }
                }
                if (reqSpic.query.primerApellido != null)
                {
                    if (reqSpic.query.primerApellido != string.Empty)
                    {
                        Query = Query + " AND Servidores.PrimerApellido LIKE '%" + reqSpic.query.primerApellido + "%'";
                    }
                }
                if (reqSpic.query.segundoApellido != null)
                {
                    if (reqSpic.query.segundoApellido != string.Empty)
                    {
                        Query = Query + " AND Servidores.SegundoApellido LIKE '%" + reqSpic.query.segundoApellido + "%'";
                    }
                }
                if (reqSpic.query.curp != null)
                {
                    if (reqSpic.query.curp != string.Empty)
                    {
                        Query = Query + " AND Servidores.CURP LIKE '%" + reqSpic.query.curp + "%'";
                    }
                }
                if (reqSpic.query.rfc != null)
                {
                    if (reqSpic.query.rfc != string.Empty)
                    {
                        Query = Query + " AND Servidores.RFC LIKE '%" + reqSpic.query.rfc + "%'";
                    }
                }
                if (reqSpic.query.institucionDependencia != null)
                {
                    if (reqSpic.query.institucionDependencia != string.Empty)
                    {
                        Query = Query + " AND Dependencia LIKE '%" + reqSpic.query.institucionDependencia + "%'";
                    }
                }

                if (reqSpic.query.TipoProcedimiento != null)
                {
                    for (var i = 0; i < reqSpic.query.TipoProcedimiento.Count; i++)
                    {

                        if (i == 0)
                        {
                            Query = Query + " AND ProcedimientoServidor.IdTipoProcedimiento IN(" + reqSpic.query.TipoProcedimiento[i] + "";
                        }
                        else
                        {
                            Query = Query + "," + reqSpic.query.TipoProcedimiento[i] + "";
                        }
                        if (i == reqSpic.query.TipoProcedimiento.Count - 1)
                        {
                            Query = Query + ")";
                        }
                    }
                }
            }
            if(reqSpic.sort != null) 
            {
                if (reqSpic.sort.nombres != null || reqSpic.sort.primerApellido != null || reqSpic.sort.segundoApellido != null || reqSpic.sort.puesto != null || reqSpic.sort.institucionDependencia != null)
                {
                    int cuenta = 0;
                    Query = Query + " ORDER BY ";
                    if (reqSpic.sort.nombres != null)
                    {
                        Query = Query + " Nombres " + reqSpic.sort.nombres;
                        cuenta = cuenta + 1;
                    }
                    if (reqSpic.sort.primerApellido != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " PrimerApellido " + reqSpic.sort.primerApellido;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", PrimerApellido " + reqSpic.sort.primerApellido;
                            cuenta = cuenta + 1;
                        }
                    }
                    if (reqSpic.sort.segundoApellido != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " SegundoApellido " + reqSpic.sort.segundoApellido;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", SegundoApellido " + reqSpic.sort.segundoApellido;
                            cuenta = cuenta + 1;
                        }
                    }
                    if (reqSpic.sort.puesto != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " Puesto " + reqSpic.sort.puesto;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", Puesto " + reqSpic.sort.puesto;
                            cuenta = cuenta + 1;
                        }
                    }
                    if (reqSpic.sort.institucionDependencia != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " Dependencia " + reqSpic.sort.institucionDependencia;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", Dependencia " + reqSpic.sort.institucionDependencia;
                            cuenta = cuenta + 1;
                        }
                    }
                }
            }

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }

        public static DataTable Area(string IdServidor)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;

            Query = @" Select TipoArea.IdTipoArea, TipoArea from TipoArea Inner Join AreaServidor on AreaServidor.IdTipoArea = TipoArea.IdTipoArea where IdServidorEnContrataciones = '" + IdServidor + "'";

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }

        public static DataTable Responsabilidad(string IdServidor)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;

            Query = @" Select NivelResponsabilidad.IdNivelResponsabilidad, NivelResponsabilidad from NivelResponsabilidad Inner Join ResponsabilidadServidor on ResponsabilidadServidor.IdNivelResponsabilidad = NivelResponsabilidad.IdNivelResponsabilidad where IdServidorEnContrataciones = '" + IdServidor + "'";

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }
        public static DataTable TipoProc(string IdServidor, reqSpic reqSpic)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;

            Query = @" Select TipoProcedimiento.IdTipoProcedimiento, TipoProcedimiento from TipoProcedimiento Inner Join ProcedimientoServidor on ProcedimientoServidor.IdTipoProcedimiento = TipoProcedimiento.IdTipoProcedimiento where IdServidorEnContrataciones = '" + IdServidor + "'";
            if (reqSpic.query != null)
            {
                if (reqSpic.query.TipoProcedimiento != null)
                {
                    for (var i = 0; i < reqSpic.query.TipoProcedimiento.Count; i++)
                    {

                        if (i == 0)
                        {
                            Query = Query + " and ProcedimientoServidor.IdTipoProcedimiento IN(" + reqSpic.query.TipoProcedimiento[i] + "";
                        }
                        else
                        {
                            Query = Query + "," + reqSpic.query.TipoProcedimiento[i] + "";
                        }
                        if (i == reqSpic.query.TipoProcedimiento.Count - 1)
                        {
                            Query = Query + ")";
                        }
                    }
                }
            }

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }
        public static DataTable TipoProcB(string IdServidor)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;

            Query = @" Select TipoProcedimiento.IdTipoProcedimiento, TipoProcedimiento from TipoProcedimiento Inner Join ProcedimientoServidor on ProcedimientoServidor.IdTipoProcedimiento = TipoProcedimiento.IdTipoProcedimiento where IdServidorEnContrataciones = '" + IdServidor + "'";

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }

        #region Segundo
        public static List<Object> CargaDependeciasSancionados()
        {
            dependSan depen = new dependSan();
            List<Object> ListaDepend = new List<Object>();
            string sqry = string.Empty;
            string errCode = "";
            sqry = @"SELECT D.Dependencia,D.SiglasDependencia,D.IdDependencia
                    FROM PDNCOAH.ServidoresPublicosSancionados SPS
                    INNER JOIN PDNCOAH.Dependencia D ON SPS.IdDependencia = D.IdDependencia
                    GROUP BY D.IdDependencia,D.SiglasDependencia,D.Dependencia";
            
            DataTable tbl = new DataTable();
            tbl = mandaQry(sqry, ref errCode);

            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                depen = new dependSan();
                depen.nombre = tbl.Rows[i][0].ToString();
                depen.siglas = tbl.Rows[i][1].ToString();
                depen.clave = tbl.Rows[i][2].ToString();
                ListaDepend.Add(depen);
            }

            return ListaDepend;

        }

        public static List<Object> CargapDependeciasSancionados()
        {
            dependSan depen = new dependSan();
            List<Object> ListaDepend = new List<Object>();
            string sqry = string.Empty;
            string errCode = "";
            sqry = @"SELECT D.Dependencia,D.SiglasDependencia,D.IdDependencia
                    FROM PDNCOAH.ParticularesSancionados PS
                    INNER JOIN PDNCOAH.Dependencia D ON PS.IdDependencia = D.IdDependencia
                    GROUP BY D.IdDependencia,D.SiglasDependencia,D.Dependencia";

            DataTable tbl = new DataTable();
            tbl = mandaQry(sqry, ref errCode);

            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                depen = new dependSan();
                depen.nombre = tbl.Rows[i][0].ToString();
                depen.siglas = tbl.Rows[i][1].ToString();
                depen.clave = tbl.Rows[i][2].ToString();
                ListaDepend.Add(depen);
            }

            return ListaDepend;

        }

        public static DataTable ssanciona(reqSan reqSanc)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;
            //Query = @"SELECT distinct SPS.IdServidorPubSancionado,SPS.FechaCaptura,SPS.Expediente,D.Dependencia,D.SiglasDependencia,D.IdDependencia,
            //S.RFC,S.CURP,S.Nombres,S.PrimerApellido,S.SegundoApellido,G.IdGenero,G.Genero,P.Puesto,S.IdPuesto,
            //SPS.AutoridadSancionadora,TF.IdTipoFalta,TF.TipoFalta,SPS.DescripFalta  ,
            //SPS.CausaMotivoHechos,SPS.URLResolucion,SPS.FechaResolucion,SPS.MontoMulta,M.IdMoneda,M.Moneda,
            //SPS.PlazoInhabilitacion,SPS.FechaInicialInhabilitacion,SPS.FechaFinalInhabilitacion,SPS.Observaciones
            //FROM ServidoresPublicosSancionados SPS
            //INNER JOIN Dependencia D ON SPS.IdDependencia=D.IdDependencia
            //INNER JOIN Servidores S ON SPS.IdServidorPublicoSancionado=S.IdServidor
            //INNER JOIN Genero G ON S.IdGenero = G.IdGenero
            //INNER JOIN Puesto P ON P.IdPuesto = S.IdPuesto
            //INNER JOIN TipoFalta TF ON SPS.IdTipoFalta=TF.IdTipoFalta
            //INNER JOIN Moneda M ON M.IdMoneda=SPS.IdMonedaMulta
            //INNER JOIN SancionServidor on SancionServidor.IdServidorPubSancionado = SancionServidor.IdServidorPubSancionado
            //WHERE SPS.IdServidorPubSancionado IS NOT NULL";

            Query = @"SELECT distinct SPS.IdServidorPubSancionado,SPS.FechaCaptura,SPS.Expediente,D.Dependencia,D.SiglasDependencia,D.IdDependencia,
            NULL AS RFC,NULL AS CURP,S.Nombres,S.PrimerApellido,S.SegundoApellido,G.IdGenero,G.Genero,P.Puesto,S.IdPuesto,
            SPS.AutoridadSancionadora,TF.IdTipoFalta,TF.TipoFalta,SPS.DescripFalta  ,
            SPS.CausaMotivoHechos,SPS.URLResolucion,SPS.FechaResolucion,SPS.MontoMulta,M.IdMoneda,M.Moneda,
            SPS.PlazoInhabilitacion,SPS.FechaInicialInhabilitacion,SPS.FechaFinalInhabilitacion,SPS.Observaciones
            FROM ServidoresPublicosSancionados SPS
            INNER JOIN Dependencia D ON SPS.IdDependencia=D.IdDependencia
            INNER JOIN Servidores S ON SPS.IdServidorPublicoSancionado=S.IdServidor
            INNER JOIN Genero G ON S.IdGenero = G.IdGenero
            INNER JOIN Puesto P ON P.IdPuesto = S.IdPuesto
            INNER JOIN TipoFalta TF ON SPS.IdTipoFalta=TF.IdTipoFalta
            INNER JOIN Moneda M ON M.IdMoneda=SPS.IdMonedaMulta
            INNER JOIN SancionServidor on SancionServidor.IdServidorPubSancionado = SancionServidor.IdServidorPubSancionado
            WHERE SPS.IdServidorPubSancionado IS NOT NULL";

            if (reqSanc.query != null)
            {
                if (reqSanc.query.id != null)
                {
                    if (reqSanc.query.id != string.Empty)
                    {
                        Query = Query + " AND SPS.IdServidorPubSancionado LIKE '%" + reqSanc.query.id + "%'";
                    }
                }
                if (reqSanc.query.nombres != null)
                {
                    if (reqSanc.query.nombres != string.Empty)
                    {
                        Query = Query + " AND S.Nombres LIKE '%" + reqSanc.query.nombres + "%'";
                    }
                }
                if (reqSanc.query.primerApellido != null)
                {
                    if (reqSanc.query.primerApellido != string.Empty)
                    {
                        Query = Query + " AND S.PrimerApellido LIKE '%" + reqSanc.query.primerApellido + "%'";
                    }
                }
                if (reqSanc.query.segundoApellido != null)
                {
                    if (reqSanc.query.segundoApellido != string.Empty)
                    {
                        Query = Query + " AND S.SegundoApellido LIKE '%" + reqSanc.query.segundoApellido + "%'";
                    }
                }
                if (reqSanc.query.curp != null)
                {
                    if (reqSanc.query.curp != string.Empty)
                    {
                        Query = Query + " AND S.CURP LIKE '%" + reqSanc.query.curp + "%'";
                    }
                }
                if (reqSanc.query.rfc != null)
                {
                    if (reqSanc.query.rfc != string.Empty)
                    {
                        Query = Query + " AND S.RFC LIKE '%" + reqSanc.query.rfc + "%'";
                    }
                }
                if (reqSanc.query.institucionDependencia != null)
                {
                    if (reqSanc.query.institucionDependencia != string.Empty)
                    {
                        Query = Query + " AND D.Dependencia LIKE '%" + reqSanc.query.institucionDependencia + "%'";
                    }
                }

                if (reqSanc.query.tipoSancion != null)
                {
                    for (var i = 0; i < reqSanc.query.tipoSancion.Count; i++)
                    {

                        if (i == 0)
                        {
                            Query = Query + " AND SancionServidor.IdTipoSancion IN('" + reqSanc.query.tipoSancion[i] + "'";
                        }
                        else
                        {
                            Query = Query + ",'" + reqSanc.query.tipoSancion[i] + "'";
                        }
                        if (i == reqSanc.query.tipoSancion.Count - 1)
                        {
                            Query = Query + ")";
                        }
                    }
                }
            }
            if (reqSanc.sort != null)
            {
                if (reqSanc.sort.rfc != null || reqSanc.sort.curp != null || reqSanc.sort.nombres != null || reqSanc.sort.primerApellido != null || reqSanc.sort.segundoApellido != null ||  reqSanc.sort.institucionDependencia != null)
                {
                    int cuenta = 0;
                    Query = Query + " ORDER BY ";
                    if (reqSanc.sort.rfc != null)
                    {
                        Query = Query + " RFC " + reqSanc.sort.rfc;
                        cuenta = cuenta + 1;
                    }
                    if (reqSanc.sort.curp != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " CURP " + reqSanc.sort.curp;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", CURP " + reqSanc.sort.curp;
                            cuenta = cuenta + 1;
                        }
                    }
                    if (reqSanc.sort.nombres != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " Nombres " + reqSanc.sort.nombres;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", Nombres " + reqSanc.sort.nombres;
                            cuenta = cuenta + 1;
                        }
                    }
                    if (reqSanc.sort.primerApellido != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " PrimerApellido " + reqSanc.sort.primerApellido;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", PrimerApellido " + reqSanc.sort.primerApellido;
                            cuenta = cuenta + 1;
                        }
                    }
                    if (reqSanc.sort.segundoApellido != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " SegundoApellido " + reqSanc.sort.segundoApellido;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", SegundoApellido " + reqSanc.sort.segundoApellido;
                            cuenta = cuenta + 1;
                        }
                    }
                   
                    if (reqSanc.sort.institucionDependencia != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " Dependencia " + reqSanc.sort.institucionDependencia;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", Dependencia " + reqSanc.sort.institucionDependencia;
                            cuenta = cuenta + 1;
                        }
                    }
                }
            }

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }

        public static DataTable psanciona(reqPSan reqSanc)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;
//            Query = @"select distinct ParticularesSancionados.IdParticularSancionado, FechaCaptura, Expediente,  Dependencia.Dependencia, Dependencia.SiglasDependencia, Dependencia.IdDependencia, 
//Particulares.NombreRazonSocial, Particulares.ObjetoSocial, Particulares.RFC, Particulares.IdTipoPersona, Particulares.Telefono, 
//Particulares.IdPais, Pais.Pais, IdEntidad, Nom_Ent, IdMunicipio, Nom_Mun,Particulares.CP, IdLocalidad, Nom_Loc, Particulares.TipoVialidad, Particulares.Vialidad, Particulares.NumeroExterior, Particulares.NumeroInterior,
//Particulares.DomExt_Calle, Particulares.DomExt_NumeroExterior, Particulares.DomExt_NumeroInterior, Particulares.DomExt_CiudadLocalidad, Particulares.DomExt_EstadoProvincia, Particulares.DomExt_Pais, PaisExt.Pais PaisExt, Particulares.DomExt_CP,
//Particulares.DirectorGeneral_Nombres, Particulares.DirectorGeneral_PrimerApellido, Particulares.DirectorGeneral_SegundoApellido, Particulares.DirectorGeneral_CURP,
//Particulares.ApoderadoLegal_Nombres, Particulares.ApoderadoLegal_PrimerApellido, Particulares.ApoderadoLegal_SegundoApellido, Particulares.ApoderadoLegal_CURP,
//ParticularesSancionados.ObjetoContrato, ParticularesSancionados.AutoridadSancionadora, ParticularesSancionados.TipoFalta, ParticularesSancionados.CausaMotivoHechos, ParticularesSancionados.Acto, ParticularesSancionados.Responsable_Nombres, ParticularesSancionados.Responsable_PrimerApellido, ParticularesSancionados.Responsable_SegundoApellido, ParticularesSancionados.SentidoResolucion, ParticularesSancionados.URLResolucion, ParticularesSancionados.FechaResolucion, ParticularesSancionados.MontoMulta, ParticularesSancionados.IdMonedaMulta, Moneda.Moneda, 
//ParticularesSancionados.PlazoInhabilitacion, ParticularesSancionados.FechaInicialInhabilitacion, ParticularesSancionados.FechaFinalInhabilitacion, ParticularesSancionados.Observaciones
//from ParticularesSancionados inner join Dependencia on Dependencia.IdDependencia = ParticularesSancionados.IdDependencia
//inner join Particulares on Particulares.IdParticular = ParticularesSancionados.IdParticular 
//left join Pais on Pais.IdPais = Particulares.IdPais 
//left join Entidades on Entidades.Cve_Ent = Particulares.IdEntidad
//left join Municipios on Municipios.Cve_Mun = Particulares.IdMunicipio AND Municipios.Cve_Ent = Particulares.IdEntidad
//left join Localidades on Localidades.Cve_Loc = Particulares.IdLocalidad AND Localidades.Cve_Mun = Particulares.IdMunicipio AND Localidades.Cve_Ent = Particulares.IdEntidad
//left join Pais PaisExt on PaisExt.IdPais = DomExt_Pais
//inner join Moneda on ParticularesSancionados.IdMonedaMulta = Moneda.IdMoneda
//inner join SancionServidor on ParticularesSancionados.IdParticularSancionado = IdServidorPubSancionado
//where IdParticularSancionado is not null";

            Query = @"select distinct ParticularesSancionados.IdParticularSancionado, FechaCaptura, Expediente,  Dependencia.Dependencia, Dependencia.SiglasDependencia, Dependencia.IdDependencia, 
Particulares.NombreRazonSocial, Particulares.ObjetoSocial, NULL AS RFC, Particulares.IdTipoPersona, Particulares.Telefono, 
Particulares.IdPais, Pais.Pais, IdEntidad, Nom_Ent, IdMunicipio, Nom_Mun,Particulares.CP, IdLocalidad, Nom_Loc, Particulares.TipoVialidad, Particulares.Vialidad, Particulares.NumeroExterior, Particulares.NumeroInterior,
Particulares.DomExt_Calle, Particulares.DomExt_NumeroExterior, Particulares.DomExt_NumeroInterior, Particulares.DomExt_CiudadLocalidad, Particulares.DomExt_EstadoProvincia, Particulares.DomExt_Pais, PaisExt.Pais PaisExt, Particulares.DomExt_CP,
Particulares.DirectorGeneral_Nombres, Particulares.DirectorGeneral_PrimerApellido, Particulares.DirectorGeneral_SegundoApellido, NULL AS DirectorGeneral_CURP,
Particulares.ApoderadoLegal_Nombres, Particulares.ApoderadoLegal_PrimerApellido, Particulares.ApoderadoLegal_SegundoApellido, NULL AS ApoderadoLegal_CURP,
ParticularesSancionados.ObjetoContrato, ParticularesSancionados.AutoridadSancionadora, ParticularesSancionados.TipoFalta, ParticularesSancionados.CausaMotivoHechos, ParticularesSancionados.Acto, ParticularesSancionados.Responsable_Nombres, ParticularesSancionados.Responsable_PrimerApellido, ParticularesSancionados.Responsable_SegundoApellido, ParticularesSancionados.SentidoResolucion, ParticularesSancionados.URLResolucion, ParticularesSancionados.FechaResolucion, ParticularesSancionados.MontoMulta, ParticularesSancionados.IdMonedaMulta, Moneda.Moneda, 
ParticularesSancionados.PlazoInhabilitacion, ParticularesSancionados.FechaInicialInhabilitacion, ParticularesSancionados.FechaFinalInhabilitacion, ParticularesSancionados.Observaciones
from ParticularesSancionados inner join Dependencia on Dependencia.IdDependencia = ParticularesSancionados.IdDependencia
inner join Particulares on Particulares.IdParticular = ParticularesSancionados.IdParticular 
left join Pais on Pais.IdPais = Particulares.IdPais 
left join Entidades on Entidades.Cve_Ent = Particulares.IdEntidad
left join Municipios on Municipios.Cve_Mun = Particulares.IdMunicipio AND Municipios.Cve_Ent = Particulares.IdEntidad
left join Localidades on Localidades.Cve_Loc = Particulares.IdLocalidad AND Localidades.Cve_Mun = Particulares.IdMunicipio AND Localidades.Cve_Ent = Particulares.IdEntidad
left join Pais PaisExt on PaisExt.IdPais = DomExt_Pais
inner join Moneda on ParticularesSancionados.IdMonedaMulta = Moneda.IdMoneda
inner join SancionServidor_Part on ParticularesSancionados.IdParticularSancionado = SancionServidor_Part.IdParticularSancionado
where SancionServidor_Part.IdParticularSancionado is not null";

            if (reqSanc.query != null)
            {
                if (reqSanc.query.id != null)
                {
                    if (reqSanc.query.id != string.Empty)
                    {
                        Query = Query + " AND ParticularesSancionados.IdParticularSancionado LIKE '%" + reqSanc.query.id + "%'";
                    }
                }
                if (reqSanc.query.nombreRazonSocial != null)
                {
                    if (reqSanc.query.nombreRazonSocial != string.Empty)
                    {
                        Query = Query + " AND NombreRazonSocial LIKE '%" + reqSanc.query.nombreRazonSocial + "%'";
                    }
                }
                if (reqSanc.query.rfc != null)
                {
                    if (reqSanc.query.rfc != string.Empty)
                    {
                        Query = Query + " AND RFC LIKE '%" + reqSanc.query.rfc + "%'";
                    }
                }
                if (reqSanc.query.institucionDependencia != null)
                {
                    if (reqSanc.query.institucionDependencia != string.Empty)
                    {
                        Query = Query + " AND Dependencia.Dependencia LIKE '%" + reqSanc.query.institucionDependencia + "%'";
                    }
                }
                if (reqSanc.query.expediente != null)
                {
                    if (reqSanc.query.expediente != string.Empty)
                    {
                        Query = Query + " AND Expediente LIKE '%" + reqSanc.query.expediente + "%'";
                    }
                }
                if (reqSanc.query.tipoPersona != null)
                {
                    if (reqSanc.query.tipoPersona != string.Empty)
                    {
                        Query = Query + " AND IdTipoPersona LIKE '%" + reqSanc.query.tipoPersona + "%'";
                    }
                }

                if (reqSanc.query.tipoSancion != null)
                {
                    for (var i = 0; i < reqSanc.query.tipoSancion.Count; i++)
                    {

                        if (i == 0)
                        {
                            Query = Query + " AND SancionServidor_Part.IdTipoSancion IN('" + reqSanc.query.tipoSancion[i] + "'";
                        }
                        else
                        {
                            Query = Query + ",'" + reqSanc.query.tipoSancion[i] + "'";
                        }
                        if (i == reqSanc.query.tipoSancion.Count - 1)
                        {
                            Query = Query + ")";
                        }
                    }
                }
            }
            if (reqSanc.sort != null)
            {
                if (reqSanc.sort.rfc != null || reqSanc.sort.nombreRazonSocial != null || reqSanc.sort.institucionDependencia != null )
                {
                    int cuenta = 0;
                    Query = Query + " ORDER BY ";
                    if (reqSanc.sort.nombreRazonSocial != null)
                    {
                        Query = Query + " NombreRazonSocial " + reqSanc.sort.nombreRazonSocial;
                        cuenta = cuenta + 1;
                    }
                    if (reqSanc.sort.rfc != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " RFC " + reqSanc.sort.rfc;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", RFC " + reqSanc.sort.rfc;
                            cuenta = cuenta + 1;
                        }
                    }
                    
                    if (reqSanc.sort.institucionDependencia != null)
                    {
                        if (cuenta == 0)
                        {
                            Query = Query + " Dependencia " + reqSanc.sort.institucionDependencia;
                            cuenta = cuenta + 1;
                        }
                        else
                        {
                            Query = Query + ", Dependencia " + reqSanc.sort.institucionDependencia;
                            cuenta = cuenta + 1;
                        }
                    }
                }
            }

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }

        public static DataTable Tiposancion(string IdServidorPubSancionado)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;

            Query = @" Select TipoSancion.IdTipoSancion, TipoSancion.TipoSancion, DescripcionSancion from SancionServidor Inner Join TipoSancion on SancionServidor.IdTipoSancion = TipoSancion.IdTipoSancion where IdServidorPubSancionado = '" + IdServidorPubSancionado + "'";

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }
        public static DataTable TiposancionP(string IdServidorPubSancionado)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;

            Query = @" Select TipoSancion.IdTipoSancion, TipoSancion.TipoSancion, DescripcionSancion from SancionServidor_Part Inner Join TipoSancion_Part TipoSancion on SancionServidor.IdTipoSancion = TipoSancion.IdTipoSancion where IdServidorPubSancionado = '" + IdServidorPubSancionado + "'";

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }

        public static DataTable Documentos(string IdServidorPubSancionado)
        {
            DataTable Result = null;
            string errCode = "";
            string Query = string.Empty;

            Query = @" Select DocumentosServSancionados.IdDocumento, TipoDocumento.TipoDocumento, Titulo, Descripcion, URL, Fecha from DocumentosServSancionados Inner Join TipoDocumento on DocumentosServSancionados.IdTipoDoc = TipoDocumento.IdTipoDoc where IdServidorSancionado = '" + IdServidorPubSancionado + "'";

            if (Query != string.Empty)
            {
                Result = mandaQry(Query, ref errCode);
            }

            return Result;
        }
        #endregion
    }

}