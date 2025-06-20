import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { GitApi, Commit } from '../../services/git-api'; // Adjusted import path

@Component({
  selector: 'app-commits',
  standalone: true,
  imports: [
    CommonModule,
    MatListModule,
    MatCardModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './commits.html',
  styleUrl: './commits.css'
})
export class Commits implements OnChanges {
  @Input() repoName: string | null = null;
  @Input() branchName: string | null = null;
  @Output() commitSelected = new EventEmitter<string>();
  commits$: Observable<Commit[]> | undefined;

  constructor(private gitApiService: GitApi) {}

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['repoName'] || changes['branchName']) && this.repoName && this.branchName) {
      this.loadCommits(this.repoName, this.branchName);
    } else {
      this.commits$ = undefined; // Clear commits if inputs are not set
    }
  }

  loadCommits(repoName: string, branchName: string): void {
    this.commits$ = this.gitApiService.getCommits(repoName, branchName);
  }

  onCommitClick(commitSha: string): void {
    this.commitSelected.emit(commitSha);
  }
}
