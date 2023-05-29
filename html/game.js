
  // Get the canvas element and its 2D rendering context
  const canvas = document.getElementById('gameCanvas');
  const context = canvas.getContext('2d');

  // Define variables for game data
  let playerX = 200; // Initial position of the player rectangle
  let score = 0;

  // Set up event listeners for player input
  document.addEventListener('keydown', handleKeyDown);
  document.addEventListener('keyup', handleKeyUp);

  // Define variables for ball data
  const ballRadius = 10; // Radius of the balls
  let balls = []; // Array to store the ball objects
  let missedBalls = 0; // Counter for missed balls


  // Function to handle player movement
  function handleKeyDown(event) {
    if (event.key === 'ArrowLeft') {
      playerX -= 5; // Move the player rectangle left
    } else if (event.key === 'ArrowRight') {
      playerX += 5; // Move the player rectangle right
    }
  }

  // Function to handle player input release
  function handleKeyUp(event) {
    // Add any logic if needed when a key is released
  }

  // Main game loop
  function gameLoop() {
    // Clear the canvas
    context.clearRect(0, 0, canvas.width, canvas.height);

    // Draw the player rectangle
    context.fillStyle = 'blue';
    context.fillRect(playerX, canvas.height - 30, 100, 20);

    // Draw the canon
    context.fillStyle = 'red';
    context.fillRect(playerX + 45, canvas.height - 35, 10, 10);

    // Display the score
    context.fillStyle = 'black';
    context.font = '20px Arial';
    context.fillText('Score: ' + score, 10, 30);

    // Update and draw the balls
    updateBalls();
    drawBalls();

    // Call the game loop recursively
    requestAnimationFrame(gameLoop);


  }

  generateBall();

  // Start the game loop
  gameLoop();


  // Function to generate random balls
  function generateBall() {
    const x = Math.random() * (canvas.width - ballRadius * 2) + ballRadius; // Random x-coordinate within the canvas
    const y = -ballRadius; // Initial y-coordinate above the canvas
    const dy = Math.random() * 2 + 1; // Random vertical speed


    balls.push({ x, y, dy }); // Add the ball object to the array
  }

  // Function to update ball positions
  function updateBalls() {
    var i=0;
    for ( i = 0; i < balls.length; i++) {
      const ball = balls[i];

      // Update the y-coordinate of the ball based on its speed
      ball.y += ball.dy;

      // Check for collisions with the player's rectangle
      if (ball.y + ballRadius >= canvas.height - 30) {
        if (ball.x >= playerX && ball.x <= playerX + 100) {
          // Ball hits the player's rectangle, reflect its movement
          ball.dy = -ball.dy;
          score += 10; // Increase score
        } else {
          // Ball falls below the player's rectangle, remove it and deduct points
          balls.splice(i, 1);
          missedBalls++;
          score -= 5; // Decrease score
        }
      }

      // Check for collisions with the canvas boundaries
      if (ball.y + ballRadius >= canvas.height) {
        // Ball falls out of the screen, remove it and deduct points
        balls.splice(i, 1);
        missedBalls++;
        score -= 5; // Decrease score
      }
    }
  }

  // Function to draw the balls on the canvas
  function drawBalls() {
    var i=0;
    for ( i = 0; i < balls.length; i++) {
      const ball = balls[i];

      // Draw the ball
      context.fillStyle = 'green';
      context.beginPath();
      context.arc(ball.x, ball.y, ballRadius, 0, Math.PI * 2);
      context.fill();
      context.closePath();
    }
  }


