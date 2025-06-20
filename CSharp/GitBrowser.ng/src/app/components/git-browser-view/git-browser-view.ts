import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { Repositories } from '../repositories/repositories';
import { Branches } from '../branches/branches';
import { Commits } from '../commits/commits';
import { Changes } from '../changes/changes';

@Component({
  selector: 'app-git-browser-view',
  standalone: true,
  imports: [CommonModule, MatCardModule, Repositories, Branches, Commits, Changes],
  templateUrl: './git-browser-view.html',
  styleUrl: './git-browser-view.css'
})
export class GitBrowserView {
  selectedRepoName: string | null = null;
  selectedBranchName: string | null = null;
  selectedCommitSha: string | null = null;

  onRepoSelected(repoName: string): void {
    this.selectedRepoName = repoName;
    this.selectedBranchName = null;
    this.selectedCommitSha = null;
  }

  onBranchSelected(branchName: string): void {
    this.selectedBranchName = branchName;
    this.selectedCommitSha = null;
  }

  onCommitSelected(commitSha: string): void {
    this.selectedCommitSha = commitSha;
  }
}
