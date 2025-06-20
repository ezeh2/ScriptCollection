import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { GitBrowserView } from './components/git-browser-view/git-browser-view'; // Ensure correct path

@Component({
  selector: 'app-root',
  standalone: true, // AppComponent should be standalone
  imports: [
    MatToolbarModule,
    GitBrowserView // Import the main view component
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'GitBrowser NG'; // Updated title
}
