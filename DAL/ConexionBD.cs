using PagosInteligentesPrueba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagosInteligentesPrueba.DAL
{
    public class ConexionBD
    {
        private readonly PagosInteligentesEntities pagosInteligentesEntities = new PagosInteligentesEntities();

        public string insertar(Users user)
        {
            try
            {
                pagosInteligentesEntities.Users.Add(user);
                pagosInteligentesEntities.SaveChanges();

                return "Exitoso: Datos guardados correctamente del usuario de pagos inteligentes";
            }
            catch (Exception)
            {
                return "Fallido: Falló al almacenar los datos del usuario pagos inteligentes";
            }
        }

        public List<Users> ListarUsuarios()
        {
            var lista = (from a in pagosInteligentesEntities.Users select a).ToList();

            return lista;
        }

        public Users userid(int id)
        {

            Users usuario = pagosInteligentesEntities.Users.Find(id);

            return usuario;

        }

        public void Guardar(Users usuario)
        {
            Users user = pagosInteligentesEntities.Users.Find(usuario.id);
            user.celular = usuario.celular;
            user.direccion = usuario.direccion;
            user.ciudad = usuario.ciudad;

            pagosInteligentesEntities.Entry(user).State = System.Data.Entity.EntityState.Modified;
            pagosInteligentesEntities.SaveChanges();
        }

        public void Eliminar(int id)
        {
            Users user = pagosInteligentesEntities.Users.Find(id);

            pagosInteligentesEntities.Users.Remove(user);
            pagosInteligentesEntities.SaveChanges();
        }

    }
}