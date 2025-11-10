using System.Diagnostics;
using WebServer.Enums;

namespace WebServer
{
    public class Timer
    {
        //Para los intervalos en segundos
        int time_delay_seconds = 1000;
        //Para los intervalos en minutos
        int time_delay_minutes = 60000;
        //Para los intervalos en horas
        int time_delay_houres = 3600000;
        //Variable que se va a guardar el intervalo de tiempo
        int time_delay = 0;
        //Objeto observador, que nos va a dar el tiempo que a pasado
        Stopwatch stopwatch = new Stopwatch();
        //funcion delegada que es responsable de cuando pase cierto tiempo
        public delegate void TickEventHandler(object source, EventArgs e);
        //Evento que se accionara cuando pase cierto tiempo
        public event TickEventHandler Tick;
        //Constructor vacio que de manera predeterminada lo configurara a un segundo
        public Timer()
        {
            time_delay = 1 * time_delay_seconds;
        }
        //Constructor con dos parametros tiempo del intervalo: tiempo a pasar para dar el tick, tipo de tiempo: la cantidad de tiempo a pasar para dar el tic
        public Timer(int Interval, TypeTime timeType)
        {
            switch (timeType)
            {
                case TypeTime.seconds:
                    time_delay = Interval * time_delay_seconds;
                    break;

                case TypeTime.minutes:
                    time_delay = Interval * time_delay_minutes;
                    break;

                case TypeTime.hours:
                    time_delay = Interval * time_delay_houres;
                    break;

            }
        }
        //Esta es la funcion que funcionara para inciar el evento
        protected virtual void onTick()
        {
            if (Tick != null)
                Tick(this, EventArgs.Empty);
            else
                Console.WriteLine("Clock was not configure");
        }
        //Esta es la funcion que iniciara el evento
        public void Start()
        {
            //Empezara a contar los milisegundos nuestro observador
            stopwatch.Start();
            //Mientras que el observador no sea menor al intervalo, seguira analizando, en caso contrario hara lo siguiente
            while (stopwatch.ElapsedMilliseconds < time_delay) ;
            //Detendra al observador
            stopwatch.Stop();
            //Accionara el evento
            onTick();
            //Se reiniciara el observador
            stopwatch.Reset();
        }
        public void Stop() => stopwatch.Stop();
    }
}