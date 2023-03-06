using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using System.Text;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Globalization;

namespace WebApplication1.Controllers
{


    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        #region carga tablas
        [HttpPost("GetServidor")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<object> GetServidor(string NombreTabla, string FechaIni, string FechaFin, string Dependencia, string Puesto, string Servidor)
        {
            List<object> Result = null;


            DataTable DTServidor = Metodos.GetServidor(NombreTabla, DateTime.Parse(FechaIni), DateTime.Parse(FechaFin), Dependencia, Puesto);

            if (DTServidor != null)
            {
                Result = new List<object>();

                foreach (DataRow Row in DTServidor.Rows)
                {
                    if (NombreTabla == "Servidores")
                        Result.Add(new { Id = Row["NOMBRE"], Name = Row["NOMBRE"] });

                }
            }
            //Result = new List<object>();
            //Result.Add(new { Id = DTServidor, Name = DTServidor });
            return Result;
        }


        [HttpPost("GetPuesto")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<object> GetPuesto(string NombreTabla, string FechaIni, string FechaFin, string Dependencia, string Puesto, string Servidor)
        {
            List<object> Result = null;


            DataTable DTPuesto = Metodos.GetPuesto(NombreTabla, DateTime.Parse(FechaIni), DateTime.Parse(FechaFin), Dependencia);

            if (DTPuesto != null)
            {
                Result = new List<object>();

                foreach (DataRow Row in DTPuesto.Rows)
                {
                    if (NombreTabla == "Servidores")
                        Result.Add(new { Id = Row["PUESTO"], Name = Row["PUESTO"] });

                }
            }

            return Result;
        }

        [HttpPost("GetDependencia")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<object> GetDependencia(string NombreTabla, string FechaIni, string FechaFin, string Dependencia, string Puesto, string Servidor)
        {
            List<object> Result = null;


            DataTable DTDependencia = Metodos.GetDependencia(NombreTabla, DateTime.Parse(FechaIni), DateTime.Parse(FechaFin));

            if (DTDependencia != null)
            {
                Result = new List<object>();

                foreach (DataRow Row in DTDependencia.Rows)
                {
                    if (NombreTabla == "Servidores")
                        Result.Add(new { Id = Row["Dependencia"], Name = Row["Dependencia"] });

                }
            }

            return Result;
        }


        [HttpPost("GetTabla")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<object> GetTabla(string NombreTabla, string FechaIni, string FechaFin, string Dependencia, string Puesto, string Servidor)
        {
            List<object> Result = null;


            DataTable Tabla = Metodos.GetTabla(NombreTabla, DateTime.Parse(FechaIni), DateTime.Parse(FechaFin), Dependencia, Puesto, Servidor);

            if (Tabla != null)
            {
                Result = new List<object>();

                foreach (DataRow Row in Tabla.Rows)
                {
                    switch (NombreTabla)
                    {
                        case "Servidores":
                            DataTable TablaArea = Metodos.Area(Row["IdServidor"].ToString());
                            DataTable TablaResponsabilidad = Metodos.Responsabilidad(Row["IdServidor"].ToString());
                            DataTable TablaTipoProc = Metodos.TipoProcB(Row["IdServidor"].ToString());
                            String Area = "", Responsab = "", TipoProc = "";
                            int cuentas = 0;
                            foreach (DataRow RowArea in TablaArea.Rows)
                            {
                                if (cuentas == 0)
                                {
                                    Area = Area + RowArea["TipoArea"].ToString();
                                }
                                else
                                {
                                    Area = Area + ", " + RowArea["TipoArea"].ToString();
                                }
                                cuentas = cuentas + 1;
                            }
                            cuentas = 0;
                            foreach (DataRow RowResponsabilidad in TablaResponsabilidad.Rows)
                            {
                                if (cuentas == 0)
                                {
                                    Responsab = Responsab + RowResponsabilidad["NivelResponsabilidad"].ToString();
                                }
                                else
                                {
                                    Responsab = Responsab + ", " + RowResponsabilidad["NivelResponsabilidad"].ToString();
                                }
                                cuentas = cuentas + 1;
                            }
                            cuentas = 0;
                            foreach (DataRow RowTipoProc in TablaTipoProc.Rows)
                            {
                                if (cuentas == 0)
                                {
                                    TipoProc = TipoProc + RowTipoProc["TipoProcedimiento"].ToString();
                                }
                                else
                                {
                                    TipoProc = TipoProc + ", " + RowTipoProc["TipoProcedimiento"].ToString();
                                }
                                cuentas = cuentas + 1;
                            }
                            Result.Add(new { Fecha = Row["Fecha"], Ejercicio = Row["Ejercicio"], Ramo = Row["Ramo"], Dependencia = Row["Dependencia"], Nombre = Row["Nombre"], Puesto = Row["Puesto"], Responsabilidad = Responsab, Tipo = TipoProc });
                            break;
                    }
                }
            }

            return Result;
        }
        #endregion


        #region Token

        [HttpPost("v1/oauth")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> Token([FromForm] string UserName, [FromForm] string Password, [FromForm] string client_id, [FromForm] string client_secret, [FromForm] string refresh_token)
        {

            var authorization = Request.Headers[HeaderNames.Authorization];
            string tokens = null;
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                tokens = headerValue.Parameter;
            }
            string Result = "";
            client client = new client();
            user user = new user();

            string Token = "";
            string RToken = "";
            string sqry = "";
            DataTable Tbl = new DataTable();
            if (refresh_token != null)
            {
                sqry = string.Format("SELECT * FROM Auth WHERE RefreshToken='{0}' and Expiracion_refresh > CURRENT_TIMESTAMP", refresh_token);
            }
            else
            {
                if (client_id == null)
                {
                    string B64Auth = Request.Headers["Authorization"].ToString();
                    B64Auth = B64Auth.Substring(6);
                    string Claves = Encoding.UTF8.GetString(Convert.FromBase64String(B64Auth));
                    string[] lisClaves = Claves.Split(":");
                    client_id = lisClaves[0];
                    client_secret = lisClaves[1];
                }
                sqry = string.Format("SELECT * FROM Usuarios_Auth WHERE USUARIO='{0}' AND PASS='{1}' AND CLIENTID='{2}' AND SECRETID='{3}'", UserName, Password, client_id, Metodos.GetSHA256(client_secret));
            }
            Tbl = Metodos.mandaQry(sqry, ref Result);

            if (Tbl.Rows.Count > 0)
            {
                var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var Charsarr = new char[8];
                var random = new Random();

                for (int i = 0; i < Charsarr.Length; i++)
                {
                    Charsarr[i] = characters[random.Next(characters.Length)];
                }

                var resultString = new String(Charsarr);
                Token = Metodos.GetSHA256(resultString);
                for (int i = 0; i < Charsarr.Length; i++)
                {
                    Charsarr[i] = characters[random.Next(characters.Length)];
                }
                resultString = new String(Charsarr);
                RToken = Metodos.GetSHA256(resultString);
                if (refresh_token != null)
                {
                    sqry = string.Format("UPDATE PDNCOAH.Auth SET Token = '{0}',FechaRegistro=CURRENT_TIMESTAMP, Expiracion= DATE_ADD(CURRENT_TIMESTAMP, INTERVAL 600 SECOND), RefreshToken = '{1}', Expiracion_refresh = DATE_ADD(CURRENT_TIMESTAMP, INTERVAL 1200 SECOND) " +
                    "WHERE RefreshToken = '{2}';", Token, RToken, refresh_token);
                }
                else
                {
                    sqry = string.Format("INSERT INTO PDNCOAH.Auth VALUES('{0}','{1}','{2}','{3}','{4}',CURRENT_TIMESTAMP, DATE_ADD(CURRENT_TIMESTAMP, INTERVAL 600 SECOND), '{5}', DATE_ADD(CURRENT_TIMESTAMP, INTERVAL 1200 SECOND)) "
                , UserName, Password, client_id, Metodos.GetSHA256(client_secret), Token, RToken);
                }
                Tbl = new DataTable();
                Tbl = Metodos.mandaQry(sqry, ref Result);


            }
            else
            {
                sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'OAUTH','','{1}','{2}')"
                , "UserName:" + UserName + " Password:" + Password + " ClientID:" + client_id + " ClientSecret:" + client_secret + " RefreshToken:" + refresh_token, "No autorizado");
                Tbl = Metodos.mandaQry(sqry, ref Result);
                return Unauthorized();
            }
            sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'OAUTH','{0}','{1}','{2}')"
            , Token, "UserName:" + UserName + " Password:" + Password + " ClientID:" + client_id + " ClientSecret:" + client_secret + " RefreshToken:" + refresh_token, "Token:" + Token + " RToken:" + RToken);
            Tbl = Metodos.mandaQry(sqry, ref Result);
            return Ok(new { token_type = "Bearer", expires_in = 600, access_token = Token, refresh_token = RToken, refresh_token_expires_in = 1200 });
        }

        [HttpPost("v1/spic/validaToken")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string ValidaToken(string Token)
        {
            string Result = "";
            string sqry = string.Format("SELECT * FROM Auth WHERE TOKEN='{0}'", Token);
            DataTable Tbl = new DataTable();
            Tbl = Metodos.mandaQry(sqry, ref Result);

            if (Tbl.Rows.Count > 0)
            {
                Result = "";
                sqry = string.Format("SELECT * FROM Auth WHERE TOKEN='{0}' AND EXPIRACION > CURRENT_TIMESTAMP", Token);
                Tbl = new DataTable();
                Tbl = Metodos.mandaQry(sqry, ref Result);

                if (Tbl.Rows.Count > 0)
                {
                    Result = "Valido";
                } else
                {
                    Result = "Expirado";
                }
            }

            return Result;
        }
        #endregion

        #region Sistema2
        [HttpPost("v1/spic")]
        [ProducesResponseType(typeof(IEnumerable<reqSpic>), 200)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 400)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 401)]

        public Object Spic(reqSpic reqSpic)
        {
            resSpic Result = new resSpic();
            var authorization = Request.Headers[HeaderNames.Authorization];
            string token = null;
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                token = headerValue.Parameter;
            }
            string resut = ValidaToken(token);

            if (resut == "Valido")
            {
                DataTable Tabla = Metodos.spic(reqSpic);
                if (reqSpic.page == null)
                {
                    reqSpic.page = 1;
                }
                if (reqSpic.pageSize == null)
                {
                    reqSpic.pageSize = 10;
                }
                if (Tabla == null)
                {
                    resError resError = new resError();
                    resError.code = "Err1";
                    resError.message = "Error al formar la consulta, revise el formato de los campos.";
                    //error default unexpected error
                    string sqry = "";
                    string RESULTADO = "";
                    DataTable Tbl = new DataTable();
                    sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SPIC','{0}','{1}','{2}')"
                    , token, JsonSerializer.Serialize(reqSpic), "Error Err1");
                    Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                    return BadRequest(resError);
                }
                else
                {
                    if (Tabla.Rows.Count == 0)
                    {
                        int cuentaini = ((Convert.ToInt32(reqSpic.page) - 1) * Convert.ToInt32(reqSpic.pageSize)) + 1;
                        int cuentafin = ((Convert.ToInt32(reqSpic.page)) * Convert.ToInt32(reqSpic.pageSize));
                        pagination pagination = new pagination();
                        pagination.totalRows = Tabla.Rows.Count;
                        pagination.pageSize = Convert.ToInt32(reqSpic.pageSize);
                        pagination.page = Convert.ToInt32(reqSpic.page);
                        pagination.hasNextPage = false;
                        Result.pagination = pagination;
                        List<respSpic> ListarespSpic = new List<respSpic>();
                        Result.results = ListarespSpic;
                        string sqry = "";
                        string RESULTADO = "";
                        DataTable Tbl = new DataTable();
                        sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SPIC','{0}','{1}','{2}')"
                        , token, JsonSerializer.Serialize(reqSpic), JsonSerializer.Serialize(Result));
                        Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                        return Result;
                    }
                    else
                    {
                        int Paginas = Convert.ToInt32(Math.Round((Convert.ToDouble(Tabla.Rows.Count) / Convert.ToDouble(Convert.ToInt32(reqSpic.pageSize))) + 0.499999));

                        if (reqSpic.page > Paginas)
                        {

                            int cuentaini = ((Convert.ToInt32(reqSpic.page) - 1) * Convert.ToInt32(reqSpic.pageSize)) + 1;
                            int cuentafin = ((Convert.ToInt32(reqSpic.page)) * Convert.ToInt32(reqSpic.pageSize));
                            pagination pagination = new pagination();
                            pagination.totalRows = Tabla.Rows.Count;
                            pagination.pageSize = Convert.ToInt32(reqSpic.pageSize);
                            pagination.page = Convert.ToInt32(reqSpic.page);
                            pagination.hasNextPage = false;
                            Result.pagination = pagination;
                            List<respSpic> ListarespSpic = new List<respSpic>();
                            Result.results = ListarespSpic;
                            string sqry = "";
                            string RESULTADO = "";
                            DataTable Tbl = new DataTable();
                            sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SPIC','{0}','{1}','{2}')"
                            , token, JsonSerializer.Serialize(reqSpic), JsonSerializer.Serialize(Result));
                            Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                            return Result;
                        }
                        else
                        {
                            int cuentaini = ((Convert.ToInt32(reqSpic.page) - 1) * Convert.ToInt32(reqSpic.pageSize)) + 1;
                            int cuentafin = ((Convert.ToInt32(reqSpic.page)) * Convert.ToInt32(reqSpic.pageSize));
                            pagination pagination = new pagination();
                            pagination.totalRows = Tabla.Rows.Count;
                            pagination.pageSize = Convert.ToInt32(reqSpic.pageSize);
                            pagination.page = Convert.ToInt32(reqSpic.page);
                            if (reqSpic.page < Paginas)
                            {
                                pagination.hasNextPage = true;
                            }
                            else
                            {
                                pagination.hasNextPage = false;
                            }

                            Result.pagination = pagination;

                            int cuentapaginacion = 1;
                            List<respSpic> ListarespSpic = new List<respSpic>();
                            foreach (DataRow Row in Tabla.Rows)
                            {
                                if (cuentapaginacion >= cuentaini && cuentapaginacion <= cuentafin)
                                {
                                    respSpic Fila = new respSpic();
                                    DataTable TablaArea = Metodos.Area(Row["IdServidorEnContrataciones"].ToString());
                                    DataTable TablaResponsabilidad = Metodos.Responsabilidad(Row["IdServidorEnContrataciones"].ToString());
                                    DataTable TablaTipoProc = Metodos.TipoProc(Row["IdServidorEnContrataciones"].ToString(), reqSpic);
                                    if (reqSpic.query.TipoProcedimiento != null)
                                    {
                                        for (var i = 0; i < reqSpic.query.TipoProcedimiento.Count; i++)
                                        {
                                            List<int> TipProdVals = new List<int> { 1, 2, 3, 4, 5 };
                                            if (!TipProdVals.Contains(reqSpic.query.TipoProcedimiento[i]))
                                            {
                                                resError resError = new resError();
                                                resError.code = "Err2";
                                                resError.message = "Error al formar la consulta, el Tipo Procedimiento es invalido.";
                                                //error default unexpected error
                                                string sqr = "";
                                                string RESULTAD = "";
                                                DataTable Tb = new DataTable();
                                                sqr = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SPIC','{0}','{1}','{2}')"
                                                , token, JsonSerializer.Serialize(reqSpic), "Error Err1");
                                                Tb = Metodos.mandaQry(sqr, ref RESULTAD);
                                                return BadRequest(resError);
                                            }

                                        }
                                    }
                                    

                                    Fila.id = Row["IdServidorEnContrataciones"].ToString();
                                    Fila.fechaCaptura = Convert.ToDateTime(Row["FechaCaptura"].ToString());
                                    Fila.ejercicioFiscal = Row["EjercicioFiscal"].ToString();
                                    Ramo Ramo = new Ramo();
                                    Ramo.clave = Convert.ToInt32(Row["IdRamo"].ToString());
                                    Ramo.valor = Row["Ramo"].ToString();
                                    Fila.Ramo = Ramo;
                                    Fila.rfc = Row["RFC"].ToString();
                                    Fila.curp = Row["CURP"].ToString();
                                    Fila.nombres = Row["Nombres"].ToString();
                                    Fila.primerApellido = Row["PrimerApellido"].ToString();
                                    Fila.segundoApellido = Row["SegundoApellido"].ToString();
                                    Genero Genero = new Genero();
                                    Genero.clave = Row["IdGenero"].ToString();
                                    Genero.valor = Row["Genero"].ToString();
                                    Fila.Genero = Genero;
                                    InstitucionDependencia InstitucionDependencia = new InstitucionDependencia();
                                    InstitucionDependencia.clave = Row["IdDependencia"].ToString();
                                    InstitucionDependencia.nombre = Row["Dependencia"].ToString();
                                    InstitucionDependencia.siglas = Row["SiglasDependencia"].ToString();
                                    Fila.InstitucionDependencia = InstitucionDependencia;
                                    Puesto Puesto = new Puesto();
                                    Puesto.nombre = Row["Puesto"].ToString();
                                    Puesto.nivel = Row["IdPuesto"].ToString();
                                    Fila.Puesto = Puesto;
                                    superiorinmediato superiorinmediato = new superiorinmediato();
                                    superiorinmediato.nombres = Row["NombresSup"].ToString();
                                    superiorinmediato.primerApellido = Row["PrimerApellidoSup"].ToString();
                                    superiorinmediato.segundoApellido = Row["SegundoApellidoSup"].ToString();
                                    superiorinmediato.rfc = Row["RFCSup"].ToString();
                                    superiorinmediato.curp = Row["CURPSup"].ToString();
                                    Puesto PuestoSup = new Puesto();
                                    PuestoSup.nivel = Row["IdPuestoSup"].ToString();
                                    PuestoSup.nombre = Row["PuestoSup"].ToString();
                                    superiorinmediato.Puesto = PuestoSup;
                                    Fila.superiorinmediato = superiorinmediato;
                                    List<TipoArea> ListTipAr = new List<TipoArea>();
                                    foreach (DataRow RowArea in TablaArea.Rows)
                                    {
                                        TipoArea FilaArea = new TipoArea();
                                        FilaArea.clave = RowArea["IdTipoArea"].ToString();
                                        FilaArea.valor = RowArea["TipoArea"].ToString();
                                        ListTipAr.Add(FilaArea);

                                    }
                                    Fila.TipoArea = ListTipAr;
                                    List<NivelResponsabilidad> ListNivelResponsabilidad = new List<NivelResponsabilidad>();
                                    foreach (DataRow RowResponsabilidad in TablaResponsabilidad.Rows)
                                    {
                                        NivelResponsabilidad FilaResponsabilidad = new NivelResponsabilidad();
                                        FilaResponsabilidad.clave = RowResponsabilidad["IdNivelResponsabilidad"].ToString();
                                        FilaResponsabilidad.valor = RowResponsabilidad["NivelResponsabilidad"].ToString();
                                        ListNivelResponsabilidad.Add(FilaResponsabilidad);
                                    }
                                    Fila.NivelResponsabilidad = ListNivelResponsabilidad;
                                    List<TipoProcedimiento> ListTipoProcedimiento = new List<TipoProcedimiento>();
                                    foreach (DataRow RowTipoProc in TablaTipoProc.Rows)
                                    {
                                        TipoProcedimiento FilaTipoProc = new TipoProcedimiento();
                                        FilaTipoProc.clave = Convert.ToInt32(RowTipoProc["IdTipoProcedimiento"].ToString());
                                        FilaTipoProc.valor = RowTipoProc["TipoProcedimiento"].ToString();
                                        ListTipoProcedimiento.Add(FilaTipoProc);
                                    }
                                    Fila.TipoProcedimiento = ListTipoProcedimiento;

                                    ListarespSpic.Add(Fila);
                                }
                                cuentapaginacion = cuentapaginacion + 1;
                            }
                            Result.results = ListarespSpic;
                            string sqry = "";
                            string RESULTADO = "";
                            DataTable Tbl = new DataTable();
                            sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SPIC','{0}','{1}','{2}')"
                            , token, JsonSerializer.Serialize(reqSpic), JsonSerializer.Serialize(Result));
                            Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                            return Result;

                        }
                    }
                }
            } else
            {

                if (resut == "Expirado")
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token ha Expirado, vuelva a obtener su token.";
                    //error default unexpected error
                    string sqry = "";
                    string RESULTADO = "";
                    DataTable Tbl = new DataTable();
                    sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SPIC','{0}','{1}','{2}')"
                    , token, JsonSerializer.Serialize(reqSpic), "El token ha Expirado, vuelva a obtener su token.");
                    Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                    return Unauthorized(resError);
                }
                else
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token no es valido.";
                    //error default unexpected error
                    string sqry = "";
                    string RESULTADO = "";
                    DataTable Tbl = new DataTable();
                    sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SPIC','{0}','{1}','{2}')"
                    , token, JsonSerializer.Serialize(reqSpic), "El token no es valido.");
                    Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                    return Unauthorized(resError);
                }
            }
        }
        // [Authorize]
        [HttpGet("v1/spic/dependencias")]
        [ProducesResponseType(typeof(IEnumerable<dependenciaslist>), 200)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 400)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 401)]
        public Object Depend()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            string token = null;
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                token = headerValue.Parameter;
            }
            string resut = ValidaToken(token);

            if (resut == "Valido")
            {
                List<Object> ListaDep = new List<Object>();
                ListaDep = Metodos.CargaDependecias();
                //dependenciaslist dep = new dependenciaslist();
                //dep.dependencias = ListaDep;

                return ListaDep;
            }
            else
            {

                if (resut == "Expirado")
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token ha Expirado, vuelva a obtener su token.";
                    //error default unexpected error
                    return Unauthorized(resError);
                }
                else
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token no es valido.";
                    //error default unexpected error
                    return Unauthorized(resError);
                }
            }
        }

        #endregion

        #region Sistema3
        [HttpPost("v1/ssancionados")]
        [ProducesResponseType(typeof(IEnumerable<resultSan>), 200)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 400)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 401)]
        public Object sancionados(reqSan req)
        {
            ssancionesResult Result = new ssancionesResult();
            var authorization = Request.Headers[HeaderNames.Authorization];
            string token = null;
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                token = headerValue.Parameter;
            }
            string resut = ValidaToken(token);

            if (resut == "Valido")
            {
                DataTable Tabla = Metodos.ssanciona(req);
                if (req.page == null)
                {
                    req.page = 1;
                }
                if (req.pageSize == null)
                {
                    req.pageSize = 10;
                }
                if (Tabla == null)
                {
                    resError resError = new resError();
                    resError.code = "Err1";
                    resError.message = "Error al formar la consulta, revise el formato de los campos.";
                    //error default unexpected error
                    string sqry = "";
                    string RESULTADO = "";
                    DataTable Tbl = new DataTable();
                    sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SSANC','{0}','{1}','{2}')"
                    , token, JsonSerializer.Serialize(req), "Error Err1");
                    Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                    return BadRequest(resError);
                }
                else
                {
                    if (Tabla.Rows.Count == 0)
                    {
                        int cuentaini = ((Convert.ToInt32(req.page) - 1) * Convert.ToInt32(req.pageSize)) + 1;
                        int cuentafin = ((Convert.ToInt32(req.page)) * Convert.ToInt32(req.pageSize));
                        paginationSan pagination = new paginationSan();
                        pagination.totalRows = Tabla.Rows.Count;
                        pagination.pageSize = Convert.ToInt32(req.pageSize);
                        pagination.page = Convert.ToInt32(req.page);
                        pagination.hasNextPage = false;
                        Result.pagination = pagination;
                        List<resultSan> ListarespSan = new List<resultSan>();
                        Result.results = ListarespSan;
                        string sqry = "";
                        string RESULTADO = "";
                        DataTable Tbl = new DataTable();
                        sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SSANC','{0}','{1}','{2}')"
                        , token, JsonSerializer.Serialize(req), JsonSerializer.Serialize(Result));
                        Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                        return Result;
                    }
                    else
                    {
                        int Paginas = Convert.ToInt32(Math.Round((Convert.ToDouble(Tabla.Rows.Count) / Convert.ToDouble(Convert.ToInt32(req.pageSize))) + 0.499999));

                        if (req.page > Paginas)
                        {

                            int cuentaini = ((Convert.ToInt32(req.page) - 1) * Convert.ToInt32(req.pageSize)) + 1;
                            int cuentafin = ((Convert.ToInt32(req.page)) * Convert.ToInt32(req.pageSize));
                            paginationSan pagination = new paginationSan();
                            pagination.totalRows = Tabla.Rows.Count;
                            pagination.pageSize = Convert.ToInt32(req.pageSize);
                            pagination.page = Convert.ToInt32(req.page);
                            pagination.hasNextPage = false;
                            Result.pagination = pagination;
                            List<resultSan> ListarespSan = new List<resultSan>();
                            Result.results = ListarespSan;
                            string sqry = "";
                            string RESULTADO = "";
                            DataTable Tbl = new DataTable();
                            sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SSANC','{0}','{1}','{2}')"
                            , token, JsonSerializer.Serialize(req), JsonSerializer.Serialize(Result));
                            Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                            return Result;
                        }
                        else
                        {
                            int cuentaini = ((Convert.ToInt32(req.page) - 1) * Convert.ToInt32(req.pageSize)) + 1;
                            int cuentafin = ((Convert.ToInt32(req.page)) * Convert.ToInt32(req.pageSize));
                            paginationSan pagination = new paginationSan();
                            pagination.totalRows = Tabla.Rows.Count;
                            pagination.pageSize = Convert.ToInt32(req.pageSize);
                            pagination.page = Convert.ToInt32(req.page);
                            if (req.page < Paginas)
                            {
                                pagination.hasNextPage = true;
                            }
                            else
                            {
                                pagination.hasNextPage = false;
                            }

                            Result.pagination = pagination;

                            int cuentapaginacion = 1;
                            List<resultSan> ListarespSpic = new List<resultSan>();
                            foreach (DataRow Row in Tabla.Rows)
                            {
                                if (cuentapaginacion >= cuentaini && cuentapaginacion <= cuentafin)
                                {
                                    resultSan Fila = new resultSan();
                                    DataTable TablaTipSan = Metodos.Tiposancion(Row["IdServidorPubSancionado"].ToString());
                                    DataTable TablaDocumentos = Metodos.Documentos(Row["IdServidorPubSancionado"].ToString());
                                    //DataTable TablaTipoProc = Metodos.TipoProc(Row["IdServidorEnContrataciones"].ToString(), req);//Modificar cuando se cambie el metodo

                                    Fila.id = Row["IdServidorPubSancionado"].ToString();
                                    Fila.fechaCaptura = Convert.ToDateTime(Row["FechaCaptura"].ToString());
                                    Fila.expediente = Row["Expediente"].ToString();//cambiar campo
                                    institucionDepenSan InstitucionDependencia = new institucionDepenSan();
                                    InstitucionDependencia.clave = Row["IdDependencia"].ToString();
                                    InstitucionDependencia.nombre = Row["Dependencia"].ToString();
                                    InstitucionDependencia.siglas = Row["SiglasDependencia"].ToString();
                                    Fila.institucionDependencia = InstitucionDependencia;

                                    servidorPublicoSan servidorPub = new servidorPublicoSan();
                                    servidorPub.rfc= Row["RFC"].ToString();
                                    servidorPub.curp=Row["CURP"].ToString();
                                    servidorPub.nombres= Row["Nombres"].ToString();
                                    servidorPub.primerApellido= Row["PrimerApellido"].ToString();
                                    servidorPub.segundoApellido= Row["SegundoApellido"].ToString();

                                    generoSan geneSan = new generoSan();
                                    geneSan.clave= Row["IdGenero"].ToString();
                                    geneSan.valor= Row["Genero"].ToString();
                                    servidorPub.genero = geneSan;
                                    servidorPub.puesto= Row["Puesto"].ToString();
                                    servidorPub.nivel= Row["IdPuesto"].ToString();
                                    
                                    Fila.servidorPublicoSancionado = servidorPub;
                                    Fila.autoridadSancionadora= Row["AutoridadSancionadora"].ToString();

                                    tipoFaltaSan tip = new tipoFaltaSan();
                                    tip.clave= Row["IdTipoFalta"].ToString();
                                    tip.valor= Row["TipoFalta"].ToString();
                                    tip.descripcion= Row["DescripFalta"].ToString();

                                    Fila.tipoFalta = tip;

                                    List<tipoSan> ListTipSan = new List<tipoSan>();
                                    foreach (DataRow Rowtipsan in TablaTipSan.Rows)
                                    {
                                        tipoSan tipsan = new tipoSan();
                                        tipsan.clave = Rowtipsan["IdTipoSancion"].ToString();
                                        tipsan.valor = Rowtipsan["TipoSancion"].ToString();
                                        tipsan.descripcion = Rowtipsan["DescripcionSancion"].ToString();
                                        ListTipSan.Add(tipsan);

                                    }
                                    Fila.tipoSancion = ListTipSan;
                                    Fila.causaMotivoHechos = Row["CausaMotivoHechos"].ToString();
                                    resolucionSan resolucionSan = new resolucionSan();
                                    resolucionSan.url = Row["URLResolucion"].ToString();
                                    resolucionSan.fechaResolucion = Convert.ToDateTime(Row["FechaResolucion"].ToString()).ToString("yyyy-MM-dd");  
                                    Fila.resolucion = resolucionSan;
                                    multaSan multaSan = new multaSan();
                                    multaSan.monto = Convert.ToDouble(Row["MontoMulta"].ToString());
                                    monedaSan monedaSan = new monedaSan();
                                    monedaSan.clave = Row["IdMoneda"].ToString();
                                    monedaSan.valor = Row["Moneda"].ToString();
                                    multaSan.moneda = monedaSan;
                                    Fila.multa = multaSan;
                                    inhabilitacionSan inhabilitacionSan = new inhabilitacionSan();
                                    inhabilitacionSan.plazo = Row["PlazoInhabilitacion"].ToString();
                                    inhabilitacionSan.fechaInicial = Convert.ToDateTime(Row["FechaInicialInhabilitacion"].ToString()).ToString("yyyy-MM-dd");
                                    inhabilitacionSan.fechaFinal = Convert.ToDateTime(Row["FechaFinalInhabilitacion"].ToString()).ToString("yyyy-MM-dd"); 
                                    Fila.inhabilitacion = inhabilitacionSan;
                                    Fila.observaciones = Row["Observaciones"].ToString();
                                    List<documentosSan> ListdocumentosSan = new List<documentosSan>();
                                    foreach (DataRow Rowtipdoc in TablaDocumentos.Rows)
                                    {
                                        documentosSan documentosSan = new documentosSan();
                                        documentosSan.id = Rowtipdoc["IdDocumento"].ToString();
                                        documentosSan.tipo = Rowtipdoc["TipoDocumento"].ToString();
                                        documentosSan.titulo = Rowtipdoc["Titulo"].ToString();
                                        documentosSan.descripcion = Rowtipdoc["Descripcion"].ToString();
                                        documentosSan.url = Rowtipdoc["URL"].ToString();
                                        documentosSan.fecha = Convert.ToDateTime(Rowtipdoc["Fecha"].ToString()).ToString("yyyy-MM-dd"); 
                                        ListdocumentosSan.Add(documentosSan);

                                    }
                                    Fila.documentos = ListdocumentosSan;
                                    ListarespSpic.Add(Fila);
                                }
                                cuentapaginacion = cuentapaginacion + 1;
                            }
                            Result.results = ListarespSpic;
                            string sqry = "";
                            string RESULTADO = "";
                            DataTable Tbl = new DataTable();
                            sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SSANC','{0}','{1}','{2}')"
                            , token, JsonSerializer.Serialize(req), JsonSerializer.Serialize(Result));
                            Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                            return Result;

                        }
                    }
                }
            } else
            {

                if (resut == "Expirado")
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token ha Expirado, vuelva a obtener su token.";
                    //error default unexpected error
                    string sqry = "";
                    string RESULTADO = "";
                    DataTable Tbl = new DataTable();
                    sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SSANC','{0}','{1}','{2}')"
                    , token, JsonSerializer.Serialize(req), "El token ha Expirado, vuelva a obtener su token.");
                    Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                    return Unauthorized(resError);
                }
                else
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token no es valido.";
                    //error default unexpected error
                    string sqry = "";
                    string RESULTADO = "";
                    DataTable Tbl = new DataTable();
                    sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'SSANC','{0}','{1}','{2}')"
                    , token, JsonSerializer.Serialize(req), "El token no es valido.");
                    Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                    return Unauthorized(resError);
                }
            }
        }

        [HttpPost("v1/psancionados")]
        [ProducesResponseType(typeof(IEnumerable<resultpSan>), 200)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 400)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 401)]
        public Object psancionados(reqPSan req)
        {
            psancionesResult Result = new psancionesResult();
            var authorization = Request.Headers[HeaderNames.Authorization];
            string token = null;
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                token = headerValue.Parameter;
            }
            string resut = ValidaToken(token);

            if (resut == "Valido")
            {
                DataTable Tabla = Metodos.psanciona(req);
                if (req.page == null)
                {
                    req.page = 1;
                }
                if (req.pageSize == null)
                {
                    req.pageSize = 10;
                }
                if (Tabla == null)
                {
                    resError resError = new resError();
                    resError.code = "Err1";
                    resError.message = "Error al formar la consulta, revise el formato de los campos.";
                    //error default unexpected error
                    string sqry = "";
                    string RESULTADO = "";
                    DataTable Tbl = new DataTable();
                    sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'PARTSAN','{0}','{1}','{2}')"
                    , token, JsonSerializer.Serialize(req), "Error Err1");
                    Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                    return BadRequest(resError);
                }
                else
                {
                    if (Tabla.Rows.Count == 0)
                    {
                        int cuentaini = ((Convert.ToInt32(req.page) - 1) * Convert.ToInt32(req.pageSize)) + 1;
                        int cuentafin = ((Convert.ToInt32(req.page)) * Convert.ToInt32(req.pageSize));
                        paginationSan pagination = new paginationSan();
                        pagination.totalRows = Tabla.Rows.Count;
                        pagination.pageSize = Convert.ToInt32(req.pageSize);
                        pagination.page = Convert.ToInt32(req.page);
                        pagination.hasNextPage = false;
                        Result.pagination = pagination;
                        List<resultpSan> ListaresppSan = new List<resultpSan>();
                        Result.results = ListaresppSan;
                        string sqry = "";
                        string RESULTADO = "";
                        DataTable Tbl = new DataTable();
                        sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'PARTSAN','{0}','{1}','{2}')"
                        , token, JsonSerializer.Serialize(req), JsonSerializer.Serialize(Result));
                        Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                        return Result;
                    }
                    else
                    {
                        int Paginas = Convert.ToInt32(Math.Round((Convert.ToDouble(Tabla.Rows.Count) / Convert.ToDouble(Convert.ToInt32(req.pageSize))) + 0.499999));

                        if (req.page > Paginas)
                        {

                            int cuentaini = ((Convert.ToInt32(req.page) - 1) * Convert.ToInt32(req.pageSize)) + 1;
                            int cuentafin = ((Convert.ToInt32(req.page)) * Convert.ToInt32(req.pageSize));
                            paginationSan pagination = new paginationSan();
                            pagination.totalRows = Tabla.Rows.Count;
                            pagination.pageSize = Convert.ToInt32(req.pageSize);
                            pagination.page = Convert.ToInt32(req.page);
                            pagination.hasNextPage = false;
                            Result.pagination = pagination;
                            List<resultpSan> ListaresppSan = new List<resultpSan>();
                            Result.results = ListaresppSan;
                            string sqry = "";
                            string RESULTADO = "";
                            DataTable Tbl = new DataTable();
                            sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'PARTSAN','{0}','{1}','{2}')"
                            , token, JsonSerializer.Serialize(req), JsonSerializer.Serialize(Result));
                            Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                            return Result;
                        }
                        else
                        {
                            int cuentaini = ((Convert.ToInt32(req.page) - 1) * Convert.ToInt32(req.pageSize)) + 1;
                            int cuentafin = ((Convert.ToInt32(req.page)) * Convert.ToInt32(req.pageSize));
                            paginationSan pagination = new paginationSan();
                            pagination.totalRows = Tabla.Rows.Count;
                            pagination.pageSize = Convert.ToInt32(req.pageSize);
                            pagination.page = Convert.ToInt32(req.page);
                            if (req.page < Paginas)
                            {
                                pagination.hasNextPage = true;
                            }
                            else
                            {
                                pagination.hasNextPage = false;
                            }

                            Result.pagination = pagination;

                            int cuentapaginacion = 1;
                            List<resultpSan> ListaresppSpic = new List<resultpSan>();
                            foreach (DataRow Row in Tabla.Rows)
                            {
                                if (cuentapaginacion >= cuentaini && cuentapaginacion <= cuentafin)
                                {
                                    resultpSan Fila = new resultpSan();
                                    DataTable TablaTipSan = Metodos.TiposancionP(Row["IdparticularSancionado"].ToString());
                                    DataTable TablaDocumentos = Metodos.Documentos(Row["IdparticularSancionado"].ToString());
                                    //DataTable TablaTipoProc = Metodos.TipoProc(Row["IdServidorEnContrataciones"].ToString(), req);//Modificar cuando se cambie el metodo

                                    Fila.id = Row["IdparticularSancionado"].ToString();
                                    Fila.fechaCaptura = Convert.ToDateTime(Row["FechaCaptura"].ToString());
                                    Fila.expediente = Row["Expediente"].ToString();//cambiar campo
                                    institucionDepenSan InstitucionDependencia = new institucionDepenSan();
                                    InstitucionDependencia.clave = Row["IdDependencia"].ToString();
                                    InstitucionDependencia.nombre = Row["Dependencia"].ToString();
                                    InstitucionDependencia.siglas = Row["SiglasDependencia"].ToString();
                                    Fila.institucionDependencia = InstitucionDependencia;

                                    particularSancionado particularSancionado = new particularSancionado();
                                    particularSancionado.nombreRazonSocial = Row["NombreRazonSocial"].ToString();
                                    particularSancionado.objetoSocial = Row["ObjetoSocial"].ToString();
                                    particularSancionado.rfc = Row["RFC"].ToString();
                                    particularSancionado.telefono = Row["Telefono"].ToString();
                                    particularSancionado.tipoPersona = Row["IdTipoPersona"].ToString();
                                    apoderadoLegal apoderadoLegal = new apoderadoLegal();
                                    apoderadoLegal.nombres = Row["ApoderadoLegal_Nombres"].ToString();
                                    apoderadoLegal.primerApellido = Row["ApoderadoLegal_PrimerApellido"].ToString();
                                    apoderadoLegal.segundoApellido = Row["ApoderadoLegal_SegundoApellido"].ToString();
                                    apoderadoLegal.curp = Row["ApoderadoLegal_CURP"].ToString();
                                    particularSancionado.apoderadoLegal = apoderadoLegal;
                                    directorGeneral directorGeneral = new directorGeneral();
                                    directorGeneral.nombres = Row["DirectorGeneral_Nombres"].ToString();
                                    directorGeneral.primerApellido = Row["DirectorGeneral_PrimerApellido"].ToString();
                                    directorGeneral.segundoApellido = Row["DirectorGeneral_SegundoApellido"].ToString();
                                    directorGeneral.curp = Row["DirectorGeneral_CURP"].ToString();
                                    particularSancionado.directorGeneral = directorGeneral;
                                    domicilioMexico domicilioMexico = new domicilioMexico();
                                    pais paismex = new pais();
                                    paismex.clave = Row["IdPais"].ToString();
                                    paismex.valor = Row["Pais"].ToString();
                                    domicilioMexico.pais = paismex;
                                    entidadFederativa entidadFederativa = new entidadFederativa();
                                    entidadFederativa.clave = Row["IdEntidad"].ToString();
                                    entidadFederativa.valor = Row["Nom_Ent"].ToString();
                                    domicilioMexico.entidadFederativa = entidadFederativa;
                                    municipio municipio = new municipio();
                                    municipio.clave = Row["IdMunicipio"].ToString();
                                    municipio.valor = Row["Nom_Mun"].ToString();
                                    domicilioMexico.municipio = municipio;
                                    localidad localidad = new localidad();
                                    localidad.clave = Row["IdLocalidad"].ToString();
                                    localidad.valor = Row["Nom_Loc"].ToString();
                                    domicilioMexico.localidad = localidad;
                                    domicilioMexico.NumeroInterior = Row["NumeroInterior"].ToString();
                                    domicilioMexico.NumeroExterior = Row["NumeroExterior"].ToString();
                                    domicilioMexico.codigoPostal = Row["CP"].ToString();
                                    vialidad vialidad = new vialidad();
                                    vialidad.clave = Row["TipoVialidad"].ToString();
                                    vialidad.valor = Row["Vialidad"].ToString();
                                    domicilioMexico.vialidad = vialidad;
                                    particularSancionado.domicilioMexico = domicilioMexico;
                                    domicilioExtranjero domicilioExtranjero  = new domicilioExtranjero();
                                    pais paisext = new pais();
                                    paisext.clave = Row["DomExt_Pais"].ToString();
                                    paisext.valor = Row["PaisExt"].ToString();
                                    domicilioExtranjero.pais = paisext;
                                    domicilioExtranjero.calle = Row["DomExt_Calle"].ToString();
                                    domicilioExtranjero.ciudadLocalidad = Row["DomExt_CiudadLocalidad"].ToString();
                                    domicilioExtranjero.codigoPostal = Row["DomExt_CP"].ToString();
                                    domicilioExtranjero.estadoProvincia = Row["DomExt_EstadoProvincia"].ToString();
                                    domicilioExtranjero.numeroExterior = Row["DomExt_NumeroExterior"].ToString();
                                    domicilioExtranjero.numeroInterior = Row["DomExt_NumeroInterior"].ToString();
                                    particularSancionado.domicilioExtranjero = domicilioExtranjero;
                                    Fila.particularSancionado = particularSancionado;
                                    Fila.objetoContrato = Row["ObjetoContrato"].ToString();
                                    Fila.autoridadSancionadora = Row["AutoridadSancionadora"].ToString();
                                    Fila.tipoFalta = Row["TipoFalta"].ToString();
                                    
                                    List<tipoSan> ListTipSan = new List<tipoSan>();
                                    foreach (DataRow Rowtipsan in TablaTipSan.Rows)
                                    {
                                        tipoSan tipsan = new tipoSan();
                                        tipsan.clave = Rowtipsan["IdTipoSancion"].ToString();
                                        tipsan.valor = Rowtipsan["TipoSancion"].ToString();
                                        tipsan.descripcion = Rowtipsan["DescripcionSancion"].ToString();
                                        ListTipSan.Add(tipsan);

                                    }
                                    Fila.tipoSancion = ListTipSan;
                                    Fila.causaMotivoHechos = Row["CausaMotivoHechos"].ToString();
                                    Fila.acto = Row["Acto"].ToString();
                                    responsableSancion responsableSancion = new responsableSancion();
                                    responsableSancion.nombres = Row["Responsable_Nombres"].ToString();
                                    responsableSancion.primerApellido = Row["Responsable_PrimerApellido"].ToString();
                                    responsableSancion.segundoApellido = Row["Responsable_SegundoApellido"].ToString();
                                    Fila.responsableSancion = responsableSancion;

                                    resolucionpSan resolucionpSan = new resolucionpSan();
                                    resolucionpSan.sentido = Row["SentidoResolucion"].ToString();
                                    resolucionpSan.url = Row["URLResolucion"].ToString();
                                    resolucionpSan.fechaResolucion = Convert.ToDateTime(Row["FechaResolucion"].ToString()).ToString("yyyy-MM-dd");
                                    Fila.resolucion = resolucionpSan;

                                    multaSan multaSan = new multaSan();
                                    multaSan.monto = Convert.ToDouble(Row["MontoMulta"].ToString());
                                    monedaSan monedaSan = new monedaSan();
                                    monedaSan.clave = Row["IdMonedaMulta"].ToString();
                                    monedaSan.valor = Row["Moneda"].ToString();
                                    multaSan.moneda = monedaSan;
                                    Fila.multa = multaSan;

                                    inhabilitacionSan inhabilitacionSan = new inhabilitacionSan();
                                    inhabilitacionSan.plazo = Row["PlazoInhabilitacion"].ToString();
                                    inhabilitacionSan.fechaInicial = Convert.ToDateTime(Row["FechaInicialInhabilitacion"].ToString()).ToString("yyyy-MM-dd");
                                    inhabilitacionSan.fechaFinal = Convert.ToDateTime(Row["FechaFinalInhabilitacion"].ToString()).ToString("yyyy-MM-dd");
                                    Fila.inhabilitacion = inhabilitacionSan;
                                    Fila.observaciones = Row["Observaciones"].ToString();

                                    List<documentosSan> ListdocumentosSan = new List<documentosSan>();
                                    foreach (DataRow Rowtipdoc in TablaDocumentos.Rows)
                                    {
                                        documentosSan documentosSan = new documentosSan();
                                        documentosSan.id = Rowtipdoc["IdDocumento"].ToString();
                                        documentosSan.tipo = Rowtipdoc["TipoDocumento"].ToString();
                                        documentosSan.titulo = Rowtipdoc["Titulo"].ToString();
                                        documentosSan.descripcion = Rowtipdoc["Descripcion"].ToString();
                                        documentosSan.url = Rowtipdoc["URL"].ToString();
                                        documentosSan.fecha = Convert.ToDateTime(Rowtipdoc["Fecha"].ToString()).ToString("yyyy-MM-dd");
                                        ListdocumentosSan.Add(documentosSan);

                                    }
                                    Fila.documentos = ListdocumentosSan;
                                    ListaresppSpic.Add(Fila);
                                }
                                cuentapaginacion = cuentapaginacion + 1;
                            }
                            Result.results = ListaresppSpic;
                            string sqry = "";
                            string RESULTADO = "";
                            DataTable Tbl = new DataTable();
                            sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'PARTSAN','{0}','{1}','{2}')"
                            , token, JsonSerializer.Serialize(req), JsonSerializer.Serialize(Result));
                            Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                            return Result;

                        }
                    }
                }
            }
            else
            {

                if (resut == "Expirado")
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token ha Expirado, vuelva a obtener su token.";
                    //error default unexpected error
                    string sqry = "";
                    string RESULTADO = "";
                    DataTable Tbl = new DataTable();
                    sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'PARTSAN','{0}','{1}','{2}')"
                    , token, JsonSerializer.Serialize(req), "El token ha Expirado, vuelva a obtener su token.");
                    Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                    return Unauthorized(resError);
                }
                else
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token no es valido.";
                    //error default unexpected error
                    string sqry = "";
                    string RESULTADO = "";
                    DataTable Tbl = new DataTable();
                    sqry = string.Format("INSERT INTO PDNCOAH.Peticiones VALUES(CURRENT_TIMESTAMP,'PARTSAN','{0}','{1}','{2}')"
                    , token, JsonSerializer.Serialize(req), "El token no es valido.");
                    Tbl = Metodos.mandaQry(sqry, ref RESULTADO);
                    return Unauthorized(resError);
                }
            }
        }


        [HttpGet("v1/ssancionados/dependencias")]
        [ProducesResponseType(typeof(IEnumerable<dependenciaslist>), 200)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 400)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 401)]
        public Object dependsancionados()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            string token = null;
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                token = headerValue.Parameter;
            }
            string resut = ValidaToken(token);

            if (resut == "Valido")
            {
                List<Object> ListaDep = new List<Object>();
                ListaDep = Metodos.CargaDependeciasSancionados();
                //dependenciaslist dep = new dependenciaslist();
                //dep.dependencias = ListaDep;

                return ListaDep;
            }
            else
            {

                if (resut == "Expirado")
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token ha Expirado, vuelva a obtener su token.";
                    //error default unexpected error
                    return Unauthorized(resError);
                }
                else
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token no es valido.";
                    //error default unexpected error
                    return Unauthorized(resError);
                }
            }
        }


        [HttpGet("v1/psancionados/dependencias")]
        [ProducesResponseType(typeof(IEnumerable<dependenciaslist>), 200)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 400)]
        [ProducesResponseType(typeof(IEnumerable<resError>), 401)]
        public Object pdependsancionados()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            string token = null;
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                token = headerValue.Parameter;
            }
            string resut = ValidaToken(token);

            if (resut == "Valido")
            {
                List<Object> ListaDep = new List<Object>();
                ListaDep = Metodos.CargapDependeciasSancionados();
                //dependenciaslist dep = new dependenciaslist();
                //dep.dependencias = ListaDep;

                return ListaDep;
            }
            else
            {

                if (resut == "Expirado")
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token ha Expirado, vuelva a obtener su token.";
                    //error default unexpected error
                    return Unauthorized(resError);
                }
                else
                {
                    resError resError = new resError();
                    resError.code = "401";
                    resError.message = "El token no es valido.";
                    //error default unexpected error
                    return Unauthorized(resError);
                }
            }
        }
        #endregion
        #region V2
        public class client
        {
            public string clientId { get; set; }
        }
        public class user
        {
            public string username { get; set; }
        }



        public class Ramo
        {
            public int clave { get; set; }
            public string valor { get; set; }
        }

        public class Genero
        {
            public string clave { get; set; }
            public string valor { get; set; }


        }
        public class InstitucionDependencia
        {
            public string nombre { get; set; }
            public string siglas { get; set; }
            public string clave { get; set; }

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public class Dependencias
        {
            public string nombre { set; get; }
            public string siglas { set; get; }
            public string clave { set; get; }
        }
        public class TipoArea
        {
            public string clave { get; set; }
            public string valor { get; set; }

        }

        public class NivelResponsabilidad
        {
            public string clave { get; set; }
            public string valor { get; set; }

        }

        public class TipoProcedimiento
        {
            public int clave { get; set; }
            public string valor { get; set; }
        }
        public class SuperiorInmediato
        {
            public string nombres { get; set; }
            public string primerApellido { get; set; }
            public string segundoApellido { get; set; }
            public string curp { get; set; }
            public string rfc { get; set; }

        }

        public class Puesto
        {
            public string nombre { get; set; }
            public string nivel { get; set; }

        }



        public class Sort
        {
            public string nombres { get; set; }

            public string primerApellido { get; set; }

            public string segundoApellido { get; set; }

            public string institucionDependencia { get; set; }

            public string puesto { get; set; }
        }



        public class Query
        {
            public string? id { get; set; }
            public string? nombres { get; set; }
            public string? primerApellido { get; set; }
            public string? segundoApellido { get; set; }
            public string? curp { get; set; }
            public string? rfc { get; set; }
            public string? institucionDependencia { get; set; }
            public List<int>? TipoProcedimiento { get; set; }//como poner unique items
            public string? rfcSolicitante { get; set; }

        }

        public class reqSpic
        {
            public Sort? sort { get; set; }
            public int? page { get; set; }
            public int? pageSize { get; set; }
            public Query query { get; set; }

        }

       
        public class pagination
        {
            public int pageSize { get; set; }
            public int page { get; set; }
            public int totalRows { get; set; }
            public Boolean hasNextPage { get; set; }

        }

        public class superiorinmediato
        {
            public string nombres { get; set; }
            public string primerApellido { get; set; }
            public string segundoApellido { get; set; }
            public string curp { get; set; }
            public string rfc { get; set; }
            public Puesto Puesto { get; set; }
        }

        public class respSpic
        {
            public string id { get; set; }
            public DateTimeOffset fechaCaptura { get; set; }
            public string ejercicioFiscal { get; set; }
            public Ramo Ramo { get; set; }
            public string rfc { get; set; }
            public string curp { get; set; }
            public string nombres { get; set; }
            public string primerApellido { get; set; }
            public string segundoApellido { get; set; }
            public Genero Genero { get; set; }
            public InstitucionDependencia InstitucionDependencia { get; set; }
            public Puesto Puesto { get; set; }
            public List<TipoArea> TipoArea { get; set; }
            public List<NivelResponsabilidad> NivelResponsabilidad { get; set; }
            public List<TipoProcedimiento> TipoProcedimiento { get; set; }
            public superiorinmediato superiorinmediato { get; set; }

        }
        public class resSpic
        {
            public pagination pagination { get; set; }
            public List<respSpic> results { get; set; }

        }

        public class dependenciaslist
        {
            public List<Dependencias> dependencias { get; set; }
        }
        public class resError

        {
            public string code { get; set; }
            public string message { get; set; }
        }


        #endregion

        #region Sancionados
        public class reqSan
        {
            public int? page { get; set; }
            public int? pageSize { get; set; }
            public SortSan? sort { get; set; }
            public QuerySan query { get; set; }

        }
        public class reqPSan
        {
            public int? page { get; set; }
            public int? pageSize { get; set; }
            public SortpSan? sort { get; set; }
            public QuerypSan query { get; set; }
            public string? rfcSolicitante { get; set; }

        }
        public class SortSan
        {
            public string rfc { get; set; }
            public string curp { get; set; }
            public string nombres { get; set; }
            public string primerApellido { get; set; }
            public string segundoApellido { get; set; }
            public string institucionDependencia { get; set; }
        }

        public class SortpSan
        {
            public string nombreRazonSocial { get; set; }
            public string rfc { get; set; }
            public string institucionDependencia { get; set; }
        }
        public class QuerySan
        {
            public string? id { get; set; }
            public string? nombres { get; set; }
            public string? primerApellido { get; set; }
            public string? segundoApellido { get; set; }
            public string? rfc { get; set; }
            public string? curp { get; set; }
            public string? institucionDependencia { get; set; }
            public List<string>? tipoSancion { get; set; }//como poner unique items
            public string? rfcSolicitante { get; set; }

        }
        public class QuerypSan
        {
            public string? id { get; set; }
            public string? nombreRazonSocial { get; set; }
            public string? rfc { get; set; }
            public string? institucionDependencia { get; set; }
            public string? expediente { get; set; }
            public List<string>? tipoSancion { get; set; }//como poner unique items
            public string? tipoPersona { get; set; }

        }
       
        public class ssancionesResult
        {
            public paginationSan? pagination { get; set; }
            public List<resultSan>? results { get; set; }
        }
        public class psancionesResult
        {
            public paginationSan? pagination { get; set; }
            public List<resultpSan>? results { get; set; }
        }
        public class paginationSan
        {
            public int? pageSize { get; set; }
            public int? page { get; set; }
            public int? totalRows { get; set; }
            public bool? hasNextPage { get; set; }
        }
        public class resultSan
        {
            public string? id { get; set; }
            public DateTimeOffset? fechaCaptura { get; set; }
            public string? expediente { get; set; }
            public institucionDepenSan? institucionDependencia { get; set; }
            public servidorPublicoSan? servidorPublicoSancionado { get; set; }
            public string? autoridadSancionadora { get; set; }
            public tipoFaltaSan? tipoFalta { get; set; }
            public List<tipoSan>? tipoSancion { get; set; }
            public string? causaMotivoHechos { get; set; }
            public resolucionSan? resolucion { get; set; }
            public multaSan? multa { get; set; }
            public inhabilitacionSan? inhabilitacion { get; set; }
            public string? observaciones { get; set; }
            public List<documentosSan>? documentos { get; set; }

        }

        public class resultpSan
        {
            public string? id { get; set; }
            public DateTimeOffset? fechaCaptura { get; set; }
            public string? expediente { get; set; }
            public institucionDepenSan? institucionDependencia { get; set; }
            public particularSancionado? particularSancionado { get; set; }
            public string? objetoContrato { get; set; }
            public string? autoridadSancionadora { get; set; }
            public string? tipoFalta { get; set; }
            public List<tipoSan>? tipoSancion { get; set; }
            public string? causaMotivoHechos { get; set; }
            public string? acto { get; set; }
            public responsableSancion? responsableSancion { get; set; }
            public resolucionpSan? resolucion { get; set; }
            public multaSan? multa { get; set; }
            public inhabilitacionSan? inhabilitacion { get; set; }
            public string? observaciones { get; set; }
            public List<documentosSan>? documentos { get; set; }

        }
        public class institucionDepenSan
        {
            public string? nombre { get; set; }
            public string? siglas { get; set; }
            public string? clave { get; set; }
        }

        public class responsableSancion
        {
            public string? nombres { get; set; }
            public string? primerApellido { get; set; }
            public string? segundoApellido { get; set; }
        }

        public class servidorPublicoSan
        {
            public string? rfc { get; set; }
            public string? curp { get; set; }
            public string? nombres { get; set; }
            public string? primerApellido { get; set; }
            public string? segundoApellido { get; set; }
            public generoSan? genero { get; set; }
            public string? puesto { get; set; }
            public string? nivel { get; set; }
        }
        public class particularSancionado
        {
            public string? nombreRazonSocial { get; set; }
            public string? objetoSocial { get; set; }
            public string? rfc { get; set; }
            public string? tipoPersona { get; set; }
            public string? telefono { get; set; }
            public domicilioMexico? domicilioMexico { get; set; }
            public domicilioExtranjero? domicilioExtranjero { get; set; }
            public directorGeneral? directorGeneral { get; set; }
            public apoderadoLegal? apoderadoLegal { get; set; }
        }
        public class domicilioMexico
        {
            public pais? pais { get; set; }
            public entidadFederativa? entidadFederativa { get; set; }
            public municipio? municipio { get; set; }
            public string? codigoPostal { get; set; }
            public localidad? localidad { get; set; }
            public vialidad? vialidad { get; set; }
            public string? NumeroExterior { get; set; }
            public string? NumeroInterior { get; set; }
        }
        public class domicilioExtranjero
        {
            public string? calle { get; set; }
            public string? numeroExterior { get; set; }
            public string? numeroInterior { get; set; }
            public string? ciudadLocalidad { get; set; }
            public string? estadoProvincia { get; set; }
            public pais? pais { get; set; }
            public string? codigoPostal { get; set; }
        }
        public class directorGeneral
        {
            public string? nombres { get; set; }
            public string? primerApellido { get; set; }
            public string? segundoApellido { get; set; }
            public string? curp { get; set; }
           
        }
        public class apoderadoLegal
        {
            public string? nombres { get; set; }
            public string? primerApellido { get; set; }
            public string? segundoApellido { get; set; }
            public string? curp { get; set; }

        }
        public class pais
        {
            public string? clave { get; set; }
            public string? valor { get; set; }
        }
        public class entidadFederativa
        {
            public string? clave { get; set; }
            public string? valor { get; set; }
        }
        public class municipio
        {
            public string? clave { get; set; }
            public string? valor { get; set; }
        }
        public class localidad
        {
            public string? clave { get; set; }
            public string? valor { get; set; }
        }
        public class vialidad
        {
            public string? clave { get; set; }
            public string? valor { get; set; }
        }
        public class generoSan
        {
            public string? clave { get; set; }
            public string? valor { get; set; }
        }
        public class tipoFaltaSan
        {
            public string? clave { get; set; }
            public string? valor { get; set; }
            public string? descripcion { get; set; }
        }
        public class tipoSan
        {
            public string? clave { get; set; }
            public string? valor { get; set; }
            public string? descripcion { get; set; }
        }
        public class resolucionpSan
        {
            public string? sentido { get; set; }
            public string? url { get; set; }
            public string? fechaResolucion { get; set; }
        }
        public class resolucionSan
        {
            public string? url { get; set; }
            public string? fechaResolucion { get; set; }
        }

        public class multaSan
        {
            public double? monto { get; set; }
            public monedaSan? moneda { get; set; }
        }

        public class monedaSan
        {
            public string? clave { get; set; }
            public string? valor { get; set; }
        }

        public class inhabilitacionSan
        {
            public string? plazo { get; set; }
            public string? fechaInicial { get; set; }
            public string? fechaFinal { get; set; }
        }
        public class documentosSan
        {
            public string? id { get; set; }
            public string? tipo { get; set; }
            public string? titulo { get; set; }
            public string? descripcion { get; set; }
            public string? url { get; set; }
            public string? fecha { get; set; }
        }

        public class dependSan
        {
            public string? nombre { get; set; }
            public string? siglas { get; set; }
            public string? clave { get; set; }

        }

        #endregion
    }
}
