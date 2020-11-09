using Domain.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MVCWEB.Models
{
    public class NotificationHub: Hub
    {
        IUtilisateurService userService;
        private readonly static ConnectionDictionnary<string> connections =
            new ConnectionDictionnary<string>();

        public NotificationHub()
        {

            userService = new UtilisateurService();
        }
        public override Task OnConnected()
        {
            //User is null then Identity and Name too.
            Console.Write("hola" + Context.User.Identity.Name + " " + Context.ConnectionId);
            
            connections.Add(Context.User.Identity.Name, Context.ConnectionId);
            return base.OnConnected();
        }
        //public void SendChatMessage(string who, string message)
        //{
        //    string name = Context.User.Identity.Name;

        //    Clients.Group(name).addChatMessage(name + ": " + message);
        //}

        //public override Task OnConnected()
        //{
        //    string name = Context.User.Identity.Name;

        //    Groups.Add(Context.ConnectionId, name);

        //    return base.OnConnected();
        //}
        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }
        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
        //public override Task OnConnected()
        //{
        //    AssignToSecurityGroup();
        //    Greet();

        //    return base.OnConnected();
        //}
        //public override Task OnConnected()
        //{
        //    var us = new Employee();
        //    us.UserName = Context.QueryString["username"];
        //    us.pseudoName=Context.ConnectionId;
        //    return base.OnConnected();
        //}
        //private void AssignToSecurityGroup()
        //{
        //    if (Context.User.Identity.IsAuthenticated)
        //        Groups.Add(Context.ConnectionId, "authenticated");
        //    else
        //        Groups.Add(Context.ConnectionId, "anonymous");
        //}

        //private void Greet()
        //{
        //    var greetedUserName = Context.User.Identity.IsAuthenticated ?
        //        Context.User.Identity.Name :
        //        "anonymous";

        //    Clients.Client(Context.ConnectionId).OnMessage(
        //        "[server]", "Welcome to the chat room, " + greetedUserName);
        //}

        ////public override Task OnDisconnected()
        ////{
        ////    RemoveFromSecurityGroups();
        ////    return base.OnDisconnected();
        ////}

        //private void RemoveFromSecurityGroups()
        //{
        //    Groups.Remove(Context.ConnectionId, "authenticated");
        //    Groups.Remove(Context.ConnectionId, "anonymous");
        //}

        //[Authorize]
        //public void SendMessage(string message)
        //{
        //    if (string.IsNullOrEmpty(message))
        //        return;

        //    BroadcastMessage(message);
        //}

        //private void BroadcastMessage(string name,string message)
        //{
        //    Clients.Caller.broadcastMessage(name, message);
        //    //var userName = Context.User.Identity.Name;

        //    //Clients.Group("authenticated").OnMessage(userName, message);

        //    //var excerpt = message.Length <= 3 ? message : message.Substring(0, 3) + "...";
        //    //Clients.Group("anonymous").OnMessage("[someone]", excerpt);
        //}
        //public override Task OnConnected()
        //{
        //    string userName = Context.User.Identity.Name;
        //    string connectionId = Context.ConnectionId;

        //    User user = userService.getBylogin(userName);

        //    //Users.GetOrAdd(userName, user);

        //    lock (user.ConnectionIds)
        //    {
        //        user.ConnectionIds.Add(connectionId);


        //        Clients.All.listUserConnected(Users.Values);

        //        string text = user.Name + " " + user.Surname + " connected!";

        //        var msg = new NotificationMessage(text, Users.Count);
        //        Clients.Client(Context.ConnectionId).addMessage(msg);
        //    }

        //    return base.OnConnected();
        //}
        //public void Send(string message)

        //{
        //    //IEmployeeService serviceEmployee = new EmployeeService();
        //    //// Call the broadcastMessage method to update clients.
        //    //Employee emp = serviceEmployee.getByLoginUser("bedoukagent");

        //    var connectionId = connections.GetConnections(Context.User.Identity.Name).FirstOrDefault();
        //    Clients.User(connectionId).addChatMessage(Context.User.Identity.Name + ": " + message);
        //    //string name = Context.User.Identity.Name;

        //    //foreach (var connectionId in connections.GetConnections(name))
        //    //{
        //    //    Clients.Client(connectionId).addChatMessage(name + ": " + message);
        //    //}
        //}
        //public Task Join()
        //{
        //    return Groups.Add(Context.ConnectionId, "foo");
        //}
        public void Broadcast(string message)

        {
            //var connectionId = connections.GetConnections(Context.User.Identity.Name).FirstOrDefault();

            //try
            //{
            //    Clients.User(connectionId).broadcastMessage(Context.User.Identity.Name + ": " + message);
            //}
            //catch (NullReferenceException e)
            //{
            //    Console.Write("hola" + Context.User.Identity.Name + " " + Context.ConnectionId,e.Message);

            //}
           
                //Clients.User(Context.User.Identity.GetUserId()).broadcastMessage(message);
            Clients.User("bedouk").broadcastMessage(message);//that's work
        }
        //public Task Send(string message)
        //{
        //    return Clients.Group("foo").addMessage(message);
        //}
    }
}
