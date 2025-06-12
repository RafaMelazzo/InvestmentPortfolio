// See https://aka.ms/new-console-template for more information

GetMainOptions();

void Option1()
{
    Console.Clear();
    Console.WriteLine("\nVocê acessou a Opção 1.");
    BackToStart();
}

void Option2()
{
    Console.Clear();
    Console.WriteLine("\nVocê acessou a Opção 2.");
    BackToStart();
}

void Option3()
{
    Console.Clear();
    Console.WriteLine("\nVocê acessou a Opção 3.");
    BackToStart();
}

void ExitProgram()
{
    Console.Clear();
    Console.WriteLine("\nSaindo do sistema...");
    Console.WriteLine("Obrigado!");
    Environment.Exit(0);
}

void BackToStart()
{
    Console.WriteLine("\nDigite qualquer tecla para retornar ao menu.");
    Console.ReadKey();
    Console.Clear();
    GetMainOptions();
}

void GetMainOptions()
{
    
    Console.WriteLine("\nOPÇÕES:");
    Console.WriteLine("1 - Opção 1");
    Console.WriteLine("2 - Opção 2");
    Console.WriteLine("3 - Opção 3");
    Console.WriteLine("00 - Sair");

    Console.Write("\nDigite o número da opção desejada: ");
    var option = int.Parse(Console.ReadLine()!);

    switch (option)
    {
        case 1:
            Option1();
            break;
        case 2:
            Option2();
            break;
        case 3:
            Option3();
            break;
        case 00:
            ExitProgram();
            break;
        default:
            Console.Clear();
            Console.WriteLine("\nOpção inválida. Por favor, tente novamente");
            GetMainOptions();
            break;
    }
}