import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Repository } from '../../git'; // Adjusted path

@Component({
  selector: 'app-repo-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './repo-list.html',
  styleUrl: './repo-list.css'
})
export class RepoListComponent {
  @Input() repositories: Repository[] = [];
  @Input() selectedRepository: Repository | null = null; // To highlight the selected one
  @Output() repositorySelected = new EventEmitter<Repository>();

  onSelect(repo: Repository): void {
    this.repositorySelected.emit(repo);
  }
}
