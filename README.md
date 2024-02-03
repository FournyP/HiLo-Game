# HiLo Game!

This repository contains a simple implementation of the HiLo game.

## What's in My Repo?

- **Client Console:** A simple console application for the game client.
- **Server Logic:** The server-side logic to manage game states.

## Getting Started

Follow these steps to build and run the HiLo game:

1. **Build the Solution**

    Run the following command in your terminal:

    ```bash
    dotnet build
    ```

2. **Run the Server**

    Start the server by executing:

    ```bash
    dotnet run --project HiLo.Server
    ```

3. **Run the Client**

    Open another terminal and start the client application. Do this twice to see the game in action:

    ```bash
    dotnet run --project HiLo.Client
    ```

## How to play ?

Once the client running, you will see this on your first terminal :
![image](https://github.com/FournyP/HiLo-Game/assets/64586968/ea521b44-18d9-4edf-991e-1132f44bd836)

And on your second terminal :
![image](https://github.com/FournyP/HiLo-Game/assets/64586968/81467fc6-6b84-4ddd-a140-8a93cc678e75)

Go back to your first terminal, enter a integer value and then press `ENTER` :
![image](https://github.com/FournyP/HiLo-Game/assets/64586968/1b813dbe-72b5-4a31-819b-ed3974043247)

Now, it's the turn of your second terminal ! You must switch to your second terminal and repeat the process :
![image](https://github.com/FournyP/HiLo-Game/assets/64586968/8b12d601-6902-4300-88fc-684f52f8706c)

PS : You can run multiple game in the same time ! Just rerun the client at least 2 time to create a new game

PS 2 : Once the game finished, you need to escape de Client using `CTRL + C`
