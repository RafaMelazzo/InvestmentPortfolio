WelcomeScreen();
return;

void WelcomeScreen()
{
    Console.Clear();
    Console.WriteLine("\nBem-vindo à sua carteira de investimentos!");
    Console.WriteLine("Para começar, selecione uma das opções abaixo.");
    GetMainOptions();
}

void GetMainOptions()
{
    string[] options = ["1", "2", "3", "99"];
    string option;
    
    do
    {
        Console.WriteLine("\nOPÇÕES:");
        Console.WriteLine("1 - Opção 1");
        Console.WriteLine("2 - Opção 2");
        Console.WriteLine("3 - Opção 3");
        Console.WriteLine("99 - Sair");

        Console.Write("\nDigite o número da opção desejada: ");
        option = Console.ReadLine()!;

        switch (option)
        {
            case "1":
                Option1();
                break;
            case "2":
                Option2();
                break;
            case "3":
                Option3();
                break;
            case "99":
                ExitProgram();
                break;
            default:
                Console.Clear();
                Console.WriteLine($"\nOpção \"{option}\" inválida.");
                // TODO: Implementar tratamento de erro para opções inválidas
                BackToStart();
                break;
        }
    } while (options.Contains(option));
}

void Option1()
{
    Console.Clear();
    Console.WriteLine("\nVocê acessou a Opção 1.");
    BackToStart();
    Console.WriteLine("\nESTE TEXTO AGORA É EXIBIDO."); // TODO: Remove this line
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
}
