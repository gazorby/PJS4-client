using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Client;

public interface ICommunication
    {

    void start(string host, int port);

    void end();

    void send(string msg);

    String receive();
}
public class Communication : ICommunication
{
    private Socket sender;
    public void end()
    {
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }

    public String receive()
    {
        byte[] bytes = new byte[1024];
        int bytesRec = sender.Receive(bytes);
        return Encoding.ASCII.GetString(bytes, 0, bytesRec);
    }

    public void send(string theMessage)
    {
        byte[] bytes = new byte[1024];
        byte[] msg = Encoding.ASCII.GetBytes(theMessage);
        int bytesSent = sender.Send(msg);
    }

    public void start(string host, int port)
    {
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[1];
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

        sender = new Socket(AddressFamily.InterNetwork,
                                   SocketType.Stream, ProtocolType.Tcp);

        sender.Connect(ipEndPoint);

        Console.WriteLine("Socket connected to {0}",
        sender.RemoteEndPoint.ToString());
    }
    
}

public class programme
{

    public static void Main()
    {
        
        Communication c = new Communication();
        //c.start("127.0.0.1", 11111);

        //while (true)
        //{
            //String message = c.receive();
//            String message = Console.ReadLine();
            Message m = new Message(/*message*/"");
            Console.WriteLine(m.getType());
            //c.send(/*message*/"");
        //}

    }
}