using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_club
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerClub computerClub = new ComputerClub(8);
            computerClub.Work();
        }
    }

    class ComputerClub
    {
        private int _money;
        private List<Computers> _computers = new List<Computers>();
        private Queue<Client> _clients = new Queue<Client>();

        public ComputerClub(int ComputersCount)
        {
            Random random = new Random();

            for(int i = 0; i < ComputersCount; i++)
            {
                _computers.Add(new Computers(random.Next(5, 15)));
            }

            CreatNewClient(25, random);
        }

        public void CreatNewClient(int count, Random random)
        {
            for(int i = 0;i < count;i++)
            {
                _clients.Enqueue(new Client(random.Next(100, 250), random));
            }
        }

        public void Work()
        {
            while(_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Баланс компьютерного клуба {_money} руб. Ждем нового клиента!");
                Console.WriteLine($"У вас новый клиент, и он хочет забронировать компьютер на {newClient.DesiredMinutes} минут.");
                ShowAllComputerState();

                Console.Write("\nВы предлагаете ему компьютер под номером: ");
                string userInput = Console.ReadLine();
                
                if(int.TryParse(userInput, out int computerNumber))
                {
                    computerNumber -= 1;

                    if(computerNumber >= 0 && computerNumber < _computers.Count)
                    {
                        if (_computers[computerNumber].IsTaken)
                        {
                            Console.WriteLine("Вы пытаетесь посадить клиента за компьютер который уже занят! Он разозлился и ушел.");

                        }
                        else
                        {
                            if (newClient.CheckSolvency(_computers[computerNumber]))
                            {
                                Console.WriteLine("Оплата прошла, клиент сел за компьютер " + computerNumber + 1);
                                _money += newClient.Pay();
                                _computers[computerNumber].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("У клиента не хватило денег и он ушел.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Вы сами не знаете за какой компьбтер посадить клиента. Он разозлился и ушел.");
                    }
                }
                else
                {
                    CreatNewClient(1, new Random());
                    Console.WriteLine("Неверный ввод, попробуйте еще раз!");
                }

                Console.WriteLine("Чтобы перейти к следующему клиенту, нажми клавишу.");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }
        }

        public void ShowAllComputerState()
        {
            Console.WriteLine("\nСписок всех компьютеров:");
            for (int i = 0; i < _computers.Count; i++)
            {
                Console.Write(i + 1 + " - ");
                _computers[i].ShowState();
            }
        }

        private void SpendOneMinute()
        {
            foreach(var computer in _computers)
            {
                computer.SpendOneMinute();
            }
        }
    }

    class Computers
    {
        private Client _client;
        private int _minutesRemain;
        public bool IsTaken
        {
            get
            {
                return _minutesRemain > 0;
            }
        }

        public int PricePerMinute {  get; private set; }

        public Computers(int PricePerMInute)
        {
            PricePerMinute = PricePerMInute;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemain = _client.DesiredMinutes;
        }

        public void BecomeEmpty()
        {
            _client = null;
        }

        public void SpendOneMinute()
        {
            _minutesRemain--;
        }

        public void ShowState()
        {
            if (IsTaken)
                Console.WriteLine($"Компьютер занят, осталось {_minutesRemain} минут.");
            else
                Console.WriteLine($"Компьютер свободен, цена за минуту: {PricePerMinute}");
        }
    }

    class Client
    {
        private int _money;
        private int _moneyToPay;
        public int DesiredMinutes { get; private set; }

        public Client(int money, Random random)
        {
            _money = money;
            DesiredMinutes = random.Next(10, 30);
        }

        public bool CheckSolvency(Computers computers)
        {
            _moneyToPay = DesiredMinutes * computers.PricePerMinute;
            if (_money >= _moneyToPay)
                return true;
            else
            {
                _moneyToPay = 0;
                return false;
            }
        }

        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;
        }
    }
}
