let currentPlayer = "X";
let board = [["", "", ""], ["", "", ""], ["", "", ""]];
let gameOver = false;

function renderBoard() {
    const boardContainer = document.getElementById("game-board");
    boardContainer.innerHTML = "";

    for (let y = 0; y < 3; y++) {
        const row = document.createElement("div");
        row.className = "d-flex";

        for (let x = 0; x < 3; x++) {
            const cell = document.createElement("button");
            cell.className = "btn btn-outline-dark m-1";
            cell.style.width = "60px";
            cell.style.height = "60px";
            cell.style.fontSize = "24px";
            cell.innerText = board[y][x];

            if (!board[y][x] && !gameOver) {
                cell.addEventListener("click", () => handleMove(x, y));
            } else {
                cell.disabled = true;
            }

            row.appendChild(cell);
        }

        boardContainer.appendChild(row);
    }
}

function handleMove(x, y) {
    if (gameOver || board[y][x]) return;

    board[y][x] = currentPlayer;
    renderBoard();

    if (checkWin(currentPlayer)) {
        endGame(currentPlayer);
        return;
    }

    if (isDraw()) {
        endGame("draw");
        return;
    }

    setTimeout(botMove, 300);
}

function botMove() {
    const empty = [];
    for (let y = 0; y < 3; y++) {
        for (let x = 0; x < 3; x++) {
            if (!board[y][x]) empty.push({ x, y });
        }
    }

    if (empty.length === 0) return;
    const move = empty[Math.floor(Math.random() * empty.length)];
    board[move.y][move.x] = "O";
    renderBoard();

    if (checkWin("O")) {
        endGame("O");
    } else if (isDraw()) {
        endGame("draw");
    }
}

function checkWin(player) {
    const winPatterns = [
        // Rows
        [[0, 0], [0, 1], [0, 2]],
        [[1, 0], [1, 1], [1, 2]],
        [[2, 0], [2, 1], [2, 2]],
        // Columns
        [[0, 0], [1, 0], [2, 0]],
        [[0, 1], [1, 1], [2, 1]],
        [[0, 2], [1, 2], [2, 2]],
        // Diagonals
        [[0, 0], [1, 1], [2, 2]],
        [[0, 2], [1, 1], [2, 0]]
    ];

    return winPatterns.some(pattern =>
        pattern.every(([y, x]) => board[y][x] === player)
    );
}

function isDraw() {
    return board.flat().every(cell => cell);
}

function endGame(result) {
    gameOver = true;
    let message = "";
    let score = { user: 0, bot: 0 };

    if (result === "X") {
        message = "✅ Vous avez gagné !";
        score.user = 1;
    } else if (result === "O") {
        message = "🤖 Le bot a gagné.";
        score.bot = 1;
    } else {
        message = "⚖️ Match nul.";
    }

    document.getElementById("game-status").innerText = message;
    document.getElementById("restart-btn").classList.remove("d-none");

    if (gameId) {
        fetch("/Game/SaveResult", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                gameId,
                userScore: score.user,
                botScore: score.bot,
                result
            })
        });
    }
}

function restartGame() {
    board = [["", "", ""], ["", "", ""], ["", "", ""]];
    gameOver = false;
    currentPlayer = "X";
    document.getElementById("game-status").innerText = "";
    document.getElementById("restart-btn").classList.add("d-none");
    renderBoard();
}

document.addEventListener("DOMContentLoaded", () => {
    renderBoard();

    const btnStart = document.getElementById("btnStartGame");
    if (btnStart) {
        btnStart.addEventListener("click", async () => {
            const response = await fetch(`/Game/StartSolo/${gameId}`, { method: "POST" });
            if (response.ok) {
                document.getElementById("game-section").classList.remove("d-none");
                restartGame();
                btnStart.classList.add("d-none");
                document.getElementById("btnStopGame").classList.remove("d-none");
            } else {
                alert("Erreur au démarrage de la partie.");
            }
        });
    }

    const restartBtn = document.getElementById("restart-btn");
    if (restartBtn) {
        restartBtn.addEventListener("click", restartGame);
    }

    const stopBtn = document.getElementById("btnStopGame");
    if (stopBtn) {
        stopBtn.addEventListener("click", async () => {
            const confirmStop = confirm("Voulez-vous vraiment arrêter la partie ?");
            if (!confirmStop) return;

            const response = await fetch(`/Game/Stop/${gameId}`, { method: "POST" });
            if (response.ok) {
                board = [["", "", ""], ["", "", ""], ["", "", ""]];
                gameOver = true;
                document.getElementById("game-section").classList.add("d-none");
                document.getElementById("game-status").innerText = "⛔ Partie arrêtée.";
                document.getElementById("btnStartGame").classList.remove("d-none");
                document.getElementById("btnStopGame").classList.add("d-none");
                document.getElementById("restart-btn").classList.add("d-none");
            } else {
                alert("Erreur lors de l'arrêt de la partie.");
            }
        });
    }
});
