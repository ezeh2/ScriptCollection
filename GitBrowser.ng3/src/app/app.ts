import { Component } from '@angular/core';
import { GitBrowserComponent } from './git-browser'; // Added

@Component({
  selector: 'app-root',
  standalone: true, // Ensure standalone is true if it wasn't already
  imports: [GitBrowserComponent], // Added GitBrowserComponent
  template: `
    <app-git-browser></app-git-browser>
  `,
  styles: [] // Assuming no global styles needed here for app-root itself
})
export class App {
  // title can be removed if not used, or keep it if needed for other purposes
  // protected title = 'GitBrowser.ng3';
}
