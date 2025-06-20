import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common'; // For pipes like DatePipe
import { Commit } from '../git'; // Adjusted path

@Component({
  selector: 'app-commit-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './commit-list.html',
  styleUrl: './commit-list.css'
})
export class CommitListComponent {
  @Input() commits: Commit[] = [];
  @Input() selectedCommit: Commit | null = null; // To highlight the selected one
  @Output() commitSelected = new EventEmitter<Commit>();

  onSelect(commit: Commit): void {
    this.commitSelected.emit(commit);
  }

  // Helper to shorten SHA for display if needed, though can also be done in template
  shortSha(sha: string): string {
    return sha.substring(0, 7);
  }
}
