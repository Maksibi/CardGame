using System;
using SFML.Learning;
using SFML.System;
using SFML.Window;
using SFML.Audio;

class CardGame : Game
{
    static string shufflesound = LoadSound("Сard_shuffle.wav");

    static int[,] cards;
    static int cardCount = 20;
    static int cardWidth = 100;
    static int cardHeight = 100;

    static int countPerLine = 5;
    static int space = 40;
    static int leftOffset = 70;
    static int topOffset = 20;

    static string[] iconsName;
    static void LoadIcons()
    {
        iconsName = new string[7];
        iconsName[0] = LoadTexture("Icon_closed.png");
        for (int i = 1; i < iconsName.Length; i++)
        {
            iconsName[i] = LoadTexture("Icon_" + i + ".png");
        }
    }

    static void Shuffle(int[] arr)
    {
        Random rand = new Random();
        for (int i = arr.Length - 1; i >= 1; i--)
        {
            int j = rand.Next(1, i + 1);
            int tmp = arr[j];
            arr[j] = arr[i];
            arr[i] = tmp;
        }
    }
    static void InitCard()
    {
        Random rnd = new Random();
        cards = new int[cardCount, 6];

        int[] iconId = new int[cards.GetLength(0)];
        int id = 0;

        for (int i = 0; i < iconId.Length; i++)
        {
            if (i % 2 == 0)
            {
                id = rnd.Next(1, 7);
            }
            iconId[i] = id;
        }
        Shuffle(iconId);
        Shuffle(iconId);
        Shuffle(iconId);

        for (int i = 0; i < iconId.Length; i++)
        {
            cards[i, 0] = 1; //state    
            cards[i, 1] = (i % countPerLine) * (cardWidth + space) + leftOffset; //posX
            cards[i, 2] = (i / countPerLine) * (cardHeight + space) + topOffset;//posY
            cards[i, 3] = cardWidth;
            cards[i, 4] = cardHeight;
            cards[i, 5] = iconId[i];
        }
    }

    static void SetStateForAllCards(int state)
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            cards[i, 0] = state;
        }
    }

    static void DrawCards()
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            if (cards[i, 0] == 1) //open
            {
                DrawSprite(iconsName[cards[i, 5]], cards[i, 1], cards[i, 2]);
            }
            if (cards[i, 0] == 0) //close
            {
                DrawSprite(iconsName[0], cards[i, 1], cards[i, 2]);
            }
        }
    }
    static int GetIndexCardByMousePosition()
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            if (MouseX >= cards[i, 1] && MouseX <= cards[i, 1] + cards[i, 3] && MouseY >= cards[i, 2] && MouseY <= cards[i, 2] + cards[i, 4])
            {
                return i;
            }
        }
        return -1;
    }


    static void Main(string[] args)
    {
        int openCardAmount = 0;
        int firstOpenCardIndex = -1;
        int secondOpenCardIndex = -1;
        float timer = 95;
        LoadIcons();
        SetFont("comic.ttf");
        InitWindow(800, 600, "CardGame");
    L1:
        ClearWindow();
        DrawText(200, 300, "Выберите уровень сложности");
        DrawText(200, 350, "1 - легко, 2 - средне, 3 - сложно.");
        DrawText(200, 400, "После выбора сложности нажмите пробел для старта игры");
        DisplayWindow();
        if (GetKey(Keyboard.Key.Num1) == true) cardCount = 8;
        if (GetKey(Keyboard.Key.Num2) == true) cardCount = 14;
        if (GetKey(Keyboard.Key.Num3) == true) cardCount = 20;
        int remainingCard = cardCount;

        if (GetKey(Keyboard.Key.Space) == true)
        {
            ClearWindow();
            InitCard();
            SetStateForAllCards(1);
            DrawCards();
            DrawText(1, 1, "Оставшееся время:" + Math.Round(timer, 0));
            DisplayWindow();
            Delay(3500);
            SetStateForAllCards(0);
            ClearWindow();

            while (true)
            {

            L4:
                DisplayWindow();
                DispatchEvents();
                if (GetKey(Keyboard.Key.R) == true)
                {
                    ClearWindow();
                    timer = 95;
                    openCardAmount = 0;
                    firstOpenCardIndex = -1;
                    secondOpenCardIndex = -1;
                    goto L1;
                }
                timer -= DeltaTime;
                Console.WriteLine(timer);
                if (timer <= 0)
                {
                    ClearWindow();
                    DrawText(200, 300, "Вы проиграли!");
                    DrawText(200, 350, "Нажмите \"R\" для перезапуска");
                    goto L4;
                }

                if (remainingCard == 0) goto L2;
                if (openCardAmount == 2)
                {
                    if (cards[firstOpenCardIndex, 5] == cards[secondOpenCardIndex, 5])
                    {
                        cards[firstOpenCardIndex, 0] = -1;
                        cards[secondOpenCardIndex, 0] = -1;
                        remainingCard -= 2;
                    }
                    else
                    {
                        cards[firstOpenCardIndex, 0] = -0;
                        cards[secondOpenCardIndex, 0] = -0;
                    }
                    firstOpenCardIndex = -1;
                    secondOpenCardIndex = -1;
                    openCardAmount = 0;

                    Delay(1000);
                }

                if (GetMouseButtonDown(0) == true)
                {
                    int index = GetIndexCardByMousePosition();
                    if (index != -1 && index != firstOpenCardIndex)
                    {
                        cards[index, 0] = 1;
                        openCardAmount++;
                        PlaySound(shufflesound);

                        if (openCardAmount == 1) firstOpenCardIndex = index;
                        if (openCardAmount == 2) secondOpenCardIndex = index;
                    }
                }
                ClearWindow();
                DrawCards();
                DrawText(1, 1, "Оставшееся время: " + Math.Round(timer, 0));
                Delay(1);
            }
        }
        else goto L1;
        L2:
        ClearWindow();
        SetFillColor(255, 255, 255);
        DrawText(200, 300, "Победа!");
        DisplayWindow();
        Delay(5000);
    }
}
