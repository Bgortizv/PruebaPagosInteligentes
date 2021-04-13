using PagosInteligentesPrueba.DAL;
using PagosInteligentesPrueba.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace PagosInteligentesPrueba.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Listado de Usuarios";
            ConexionBD conexion = new ConexionBD();
            List<Users> listaUsuarios = conexion.ListarUsuarios();
            ViewBag.lista = listaUsuarios;
            return View();
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Carga de archivo";

            return View();
        }


        [HttpPost]
        public ActionResult Contact(HttpPostedFileBase archivoCargado)
        {
            ViewBag.Message = "Carga el archivo deseado.";
            try
            {
                ConexionBD conectionDB = new ConexionBD();
                //ruta de archivo
                string filePath = string.Empty;
                if (archivoCargado != null)
                {
                    //Creacion de la ruta donde se va a almacenar el archivo
                    string path = Server.MapPath("~/App_Data/archivosCSV/");

                    //si la ruta no existe creamos una.

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(archivoCargado.FileName);
                    string extension = Path.GetExtension(archivoCargado.FileName);

                    //Guardo en el servidor
                    archivoCargado.SaveAs(filePath);

                    //Lectura del archivo desde el servidor
                    string datosCVS = System.IO.File.ReadAllText(filePath);

                    //Llenamos la lista de usuarios
                    foreach (string row in datosCVS.Split('\n'))
                    {


                        Users usuario = new Users();
                        //verifico que el row no venga vacío
                        if (!string.IsNullOrEmpty(row))
                        {
                            usuario.id = Convert.ToInt32(row.Split(';')[0]);
                            usuario.nombres = row.Split(';')[1];
                            usuario.apellidos = row.Split(';')[2];
                            usuario.identificacion = row.Split(';')[3];
                            usuario.celular = row.Split(';')[4]; ;
                            usuario.direccion = row.Split(';')[5];
                            usuario.ciudad = row.Split(';')[6];
                            usuario.email = row.Split(';')[7];

                            string resultado = conectionDB.insertar(usuario);
                            EnviarCorreos(usuario, usuario.email.Substring(0, usuario.email.Length - 1), resultado);
                        }
        
                    }
                }
                return View();
            }
            catch (Exception)
            {

                throw;
            }
            

            
        }

        public void EnviarCorreos(Users usuario, string correo, string resultado)
        {
            var fromAddress = new MailAddress("bgortizv@gmail.com", "From Name");
            var toAddress = new MailAddress(correo, "To Name");
            const string fromPassword = "QcGr80vdfJ";
            const string subject = "Inserción de Datos";
            string body = resultado;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }


        public ActionResult Editar(int id)
        {
            ConexionBD conexionBD = new ConexionBD();
            Users usuario = conexionBD.userid(id);
            return View(usuario);
        }

        [HttpPost]
        public ActionResult Editar(Users usuario)
        {
            ConexionBD conexionBD = new ConexionBD();
            conexionBD.Guardar(usuario);
            ConexionBD conexion = new ConexionBD();
            List<Users> listaUsuarios = conexion.ListarUsuarios();
            ViewBag.lista = listaUsuarios;

            return View("About");
        }

        public ActionResult Eliminar(int id)
        {
            ConexionBD conexion = new ConexionBD();
            conexion.Eliminar(id);

            List<Users> listaUsuarios = conexion.ListarUsuarios();

            ViewBag.lista = listaUsuarios;


            return View("About");
        }

        public ActionResult ExportarArchivo()
        {
            string path = Server.MapPath("~/App_Data/archivosCSV/");
            string texto = "";
            ConexionBD conexionBD = new ConexionBD();
            List<Users> listaUsuarios = conexionBD.ListarUsuarios();
            foreach (var item in listaUsuarios)
            {
                texto += item.id.ToString() + ";" + item.nombres.ToString() + ";" + item.apellidos.ToString() + 
                            ";" + item.identificacion.ToString() + ";" + item.celular.ToString() + ";" 
                            + item.direccion.ToString() + ";" + item.ciudad.ToString() + ";" + item.email.ToString() + "\n";
            }           

            using (StreamWriter file = new StreamWriter(path+ "ArchivoDescarga.csv", true))
            {
                file.WriteLine(texto); //se agrega información al documento
                file.Close();
            }


            byte[] fileBytes = System.IO.File.ReadAllBytes(path + "ArchivoDescarga.csv");
            MemoryStream ms = new MemoryStream(fileBytes, 0, 0, true, true);
            Response.AddHeader("content-disposition", "attachment;filename= NombreArchivo.csv");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "text/csv");

        }

    }

}