## Testing CORS Configuration

To test the CORS configuration for GitBrowser, follow these steps:

1.  **Run the GitBrowser Application:**
    *   Open a terminal or command prompt.
    *   Navigate to the `CSharp/GitBrowser` directory.
    *   Run the command `dotnet run`.
    *   The application should start, typically listening on a port like `https://localhost:5001` or `http://localhost:5000`. Note the exact URL.

2.  **Create a Simple Web Page (or use an existing frontend app on port 4200):**
    *   Create an HTML file (e.g., `test_cors.html`) on your local machine with the following content:

    ```html
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>CORS Test</title>
    </head>
    <body>
        <h1>CORS Test for GitBrowser</h1>
        <button onclick="fetchData()">Fetch Repos</button>
        <pre id="output"></pre>

        <script>
            async function fetchData() {
                const outputElement = document.getElementById('output');
                const gitBrowserApiUrl = 'REPLACE_WITH_GITBROWSER_API_URL/api/repos'; // e.g., http://localhost:5000/api/repos

                outputElement.textContent = 'Fetching...';

                try {
                    // Make sure the GitBrowser application is running and accessible at gitBrowserApiUrl
                    // Ensure that there is at least one repository configured in appsettings.json for GitBrowser
                    // For example:
                    // "GitSettings": {
                    //   "Repositories": [
                    //     {
                    //       "Name": "Test Repo",
                    //       "Path": "/path/to/your/test/git/repo"
                    //     }
                    //   ]
                    // }

                    const response = await fetch(gitBrowserApiUrl, {
                        method: 'GET',
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    });

                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status} - ${response.statusText}`);
                    }

                    const data = await response.json();
                    outputElement.textContent = JSON.stringify(data, null, 2);
                } catch (error) {
                    outputElement.textContent = `Error: ${error.message}\n\nCheck the browser console (F12) for more details (e.g., CORS errors).`;
                    console.error('Fetch error:', error);
                }
            }
        </script>
    </body>
    </html>
    ```

3.  **Serve the HTML file from `http://localhost:4200`:**
    *   You can use a simple HTTP server for this. If you have Node.js installed, you can use `npx serve` or `http-server`.
    *   Open a terminal in the directory where you saved `test_cors.html`.
    *   Run a command like `npx serve -p 4200 .` (if using `serve`) or `http-server -p 4200 .` (if using `http-server`).
    *   This will serve the HTML file on `http://localhost:4200/test_cors.html`.

4.  **Perform the Test:**
    *   Open your web browser and navigate to `http://localhost:4200/test_cors.html`.
    *   Open the browser's developer console (usually by pressing F12) and go to the "Network" and "Console" tabs.
    *   In the `test_cors.html` page, replace `REPLACE_WITH_GITBROWSER_API_URL` in the script with the actual URL where your GitBrowser application is running (e.g., `http://localhost:5000` or `https://localhost:5001`). *Make sure the path `/api/repos` is correct based on `GitApiController.cs`.*
    *   Click the "Fetch Repos" button.

5.  **Verify the Result:**
    *   **Success:** If CORS is configured correctly, you will see a list of repositories (or an empty array if no repos are configured in GitBrowser's `appsettings.json`) displayed on the page. There should be no CORS-related errors in the browser console.
    *   **Failure:** If there's a CORS issue, the `fetch` call will fail, and you'll likely see an error message in the browser console similar to: `Access to fetch at '...' from origin 'http://localhost:4200' has been blocked by CORS policy...` The `outputElement` on the page will also display an error.

This manual test will confirm that an application running on `http://localhost:4200` can successfully make requests to the GitBrowser API.
