import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; // Required for *ngIf, *ngFor, etc.
import { GitService, Repository, Branch, Commit, CommitChange } from './git'; // Adjusted import path

import { RepoListComponent } from './repo-list/repo-list';
import { BranchListComponent } from './branch-list/branch-list';
import { CommitListComponent } from './commit-list/commit-list';
import { ChangeListComponent } from './change-list/change-list';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RepoListComponent,
    BranchListComponent,
    CommitListComponent,
    ChangeListComponent
  ],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent implements OnInit {
  title = 'GitBrowser NG2';

  repositories: Repository[] = [];
  selectedRepository: Repository | null = null;
  branches: Branch[] = [];
  selectedBranch: Branch | null = null;
  commits: Commit[] = [];
  selectedCommit: Commit | null = null;
  changes: CommitChange[] = [];
  errorMessage: string | null = null;

  isLoadingRepositories = false;
  isLoadingBranches = false;
  isLoadingCommits = false;
  isLoadingChanges = false;

  constructor(private gitService: GitService) {}

  ngOnInit(): void {
    this.fetchRepositories();
  }

  fetchRepositories(): void {
    this.isLoadingRepositories = true;
    this.errorMessage = null;
    this.repositories = []; // Clear previous results
    this.gitService.getRepositories().subscribe({
      next: (data) => {
        this.repositories = data;
        this.isLoadingRepositories = false;
      },
      error: (err) => {
        this.errorMessage = `Error fetching repositories: ${err.message || 'Unknown error'}`;
        console.error(err);
        this.isLoadingRepositories = false;
      }
    });
  }

  onSelectRepository(repo: Repository): void {
    this.selectedRepository = repo;
    this.selectedBranch = null;
    this.selectedCommit = null;
    this.branches = [];
    this.commits = [];
    this.changes = [];
    this.errorMessage = null;

    if (repo) {
      this.isLoadingBranches = true;
      this.gitService.getBranches(repo.name).subscribe({
        next: (data) => {
          this.branches = data;
          this.isLoadingBranches = false;
        },
        error: (err) => {
          this.errorMessage = `Error fetching branches for ${repo.name}: ${err.message || 'Unknown error'}`;
          console.error(err);
          this.isLoadingBranches = false;
        }
      });
    }
  }

  onSelectBranch(branch: Branch): void {
    this.selectedBranch = branch;
    this.selectedCommit = null;
    this.commits = [];
    this.changes = [];
    this.errorMessage = null;

    if (this.selectedRepository && branch) {
      this.isLoadingCommits = true;
      this.gitService.getCommits(this.selectedRepository.name, branch.name).subscribe({
        next: (data) => {
          this.commits = data;
          this.isLoadingCommits = false;
        },
        error: (err) => {
          this.errorMessage = `Error fetching commits for ${this.selectedRepository?.name} / ${branch.name}: ${err.message || 'Unknown error'}`;
          console.error(err);
          this.isLoadingCommits = false;
        }
      });
    }
  }

  onSelectCommit(commit: Commit): void {
    this.selectedCommit = commit;
    this.changes = [];
    this.errorMessage = null;

    if (this.selectedRepository && commit) {
      this.isLoadingChanges = true;
      this.gitService.getCommitChanges(this.selectedRepository.name, commit.sha).subscribe({
        next: (data) => {
          this.changes = data;
          this.isLoadingChanges = false;
        },
        error: (err) => {
          this.errorMessage = `Error fetching changes for commit ${commit.sha}: ${err.message || 'Unknown error'}`;
          console.error(err);
          this.isLoadingChanges = false;
        }
      });
    }
  }
}
