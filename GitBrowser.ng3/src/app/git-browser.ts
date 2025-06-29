import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common'; // Import DatePipe if you use it in template
import { HttpClient, provideHttpClient, withFetch } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { GitRepo, GitBranch, GitCommit, GitCommitChange } from './git-data.models';

@Component({
  selector: 'app-git-browser',
  standalone: true,
  imports: [CommonModule], // CommonModule provides *ngFor, *ngIf, DatePipe etc.
  template: `
    <div class="container">
      <header>
        <h1>Git Browser .ng3</h1>
      </header>

      <main class="content-area">
        <!-- Repositories List -->
        <section class="column">
          <h2>Repositories <span *ngIf="loadingRepos" class="loading-indicator">(Loading...)</span></h2>
          <ul *ngIf="!loadingRepos && repositories.length > 0" class="item-list">
            <li *ngFor="let repo of repositories"
                [class.selected]="repo.name === selectedRepoName"
                (click)="onSelectRepo(repo.name)"
                tabindex="0" (keydown.enter)="onSelectRepo(repo.name)">
              {{ repo.name }}
            </li>
          </ul>
          <p *ngIf="!loadingRepos && repositories.length === 0" class="no-data-message">No repositories found.</p>
        </section>

        <!-- Branches List -->
        <section class="column" *ngIf="selectedRepoName">
          <h2>Branches: {{ selectedRepoName }} <span *ngIf="loadingBranches" class="loading-indicator">(Loading...)</span></h2>
          <ul *ngIf="!loadingBranches && branches.length > 0" class="item-list">
            <li *ngFor="let branch of branches"
                [class.selected]="branch.name === selectedBranchName"
                (click)="onSelectBranch(branch.name)"
                tabindex="0" (keydown.enter)="onSelectBranch(branch.name)">
              {{ branch.name }} <span *ngIf="branch.isRemote" class="branch-type">(remote)</span>
            </li>
          </ul>
          <p *ngIf="!loadingBranches && branches.length === 0 && !loadingRepos" class="no-data-message">No branches found.</p>
        </section>

        <!-- Commits List -->
        <section class="column" *ngIf="selectedBranchName">
          <h2>Commits: {{ selectedBranchName }} <span *ngIf="loadingCommits" class="loading-indicator">(Loading...)</span></h2>
          <ul *ngIf="!loadingCommits && commits.length > 0" class="item-list commit-list">
            <li *ngFor="let commit of commits"
                [class.selected]="commit.sha === selectedCommitSha"
                (click)="onSelectCommit(commit.sha)"
                tabindex="0" (keydown.enter)="onSelectCommit(commit.sha)">
              <div class="commit-sha">{{ commit.sha.substring(0, 7) }}</div>
              <div class="commit-message">{{ commit.messageShort }}</div>
              <div class="commit-details">
                <span class="author">{{ commit.author }}</span> on <span class="date">{{ commit.authorDate | date:'yyyy-MM-dd HH:mm' }}</span>
              </div>
            </li>
          </ul>
          <p *ngIf="!loadingCommits && commits.length === 0 && !loadingBranches" class="no-data-message">No commits found.</p>
        </section>

        <!-- Changes List -->
        <section class="column" *ngIf="selectedCommitSha">
          <h2>Changes: {{ selectedCommitSha.substring(0, 7) }} <span *ngIf="loadingChanges" class="loading-indicator">(Loading...)</span></h2>
          <ul *ngIf="!loadingChanges && changes.length > 0" class="item-list change-list">
            <li *ngFor="let change of changes">
              <span class="status-badge status-{{ change.status | lowercase }}">{{ change.status }}</span>
              <span class="file-name">{{ change.fileName }}</span>
            </li>
          </ul>
          <p *ngIf="!loadingChanges && changes.length === 0 && !loadingCommits" class="no-data-message">No changes found.</p>
        </section>
      </main>
    </div>
  `,
  styles: [
    `
    :host {
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol";
      font-size: 14px;
      color: #333;
      box-sizing: border-box;
      -webkit-font-smoothing: antialiased;
      -moz-osx-font-smoothing: grayscale;
    }
    .container { padding: 0 20px; }
    header { padding: 20px 0; border-bottom: 1px solid #ccc; margin-bottom: 20px; }
    header h1 { margin: 0; font-size: 24px; }

    .content-area {
      display: flex;
      flex-wrap: nowrap; /* Prevent wrapping to new line, allow horizontal scroll if needed */
      gap: 20px;
      overflow-x: auto; /* Add horizontal scroll for columns */
    }

    .column {
      flex: 1 1 300px; /* Flex grow, flex shrink, basis */
      min-width: 280px; /* Minimum width before shrinking */
      max-width: 400px; /* Maximum width */
      border: 1px solid #e0e0e0;
      padding: 15px;
      border-radius: 8px;
      background-color: #f9f9f9;
      height: calc(100vh - 150px); /* Example height, adjust as needed */
      overflow-y: auto;
    }
    .column h2 {
      font-size: 18px;
      margin-top: 0;
      margin-bottom: 15px;
      color: #1a237e; /* Dark blue */
      border-bottom: 2px solid #c5cae9; /* Light blue */
      padding-bottom: 8px;
    }
    .loading-indicator { font-size: 0.9em; color: #555; font-style: italic; }
    .no-data-message { color: #757575; font-style: italic; text-align: center; margin-top: 20px; }

    .item-list { list-style-type: none; padding: 0; margin: 0; }
    .item-list li {
      padding: 10px 12px;
      margin-bottom: 8px;
      border-radius: 6px;
      cursor: pointer;
      border: 1px solid #ddd;
      background-color: #fff;
      transition: background-color 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
    }
    .item-list li:hover {
      background-color: #e8eaf6; /* Light indigo */
      border-color: #9fa8da; /* Indigo */
    }
    .item-list li.selected {
      background-color: #3f51b5; /* Indigo */
      color: white;
      border-color: #303f9f; /* Dark Indigo */
      font-weight: bold;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .item-list li:focus {
      outline: 2px solid #3f51b5;
      outline-offset: 2px;
    }

    .branch-type { font-size: 0.85em; color: #4caf50; /* Green */ }
    .item-list li.selected .branch-type { color: #e8f5e9; /* Light Green */ }

    .commit-list li { display: flex; flex-direction: column; }
    .commit-sha { font-weight: bold; color: #0d47a1; /* Dark Blue */ font-size: 0.9em; }
    .commit-message { margin: 4px 0; }
    .commit-details { font-size: 0.8em; color: #424242; } /* Dark Grey */
    .commit-details .author { font-style: italic; }
    .commit-details .date { color: #616161; } /* Grey */
    .item-list li.selected .commit-sha,
    .item-list li.selected .commit-message,
    .item-list li.selected .commit-details,
    .item-list li.selected .commit-details .author,
    .item-list li.selected .commit-details .date {
      color: white;
    }

    .change-list li { display: flex; align-items: center; gap: 10px; }
    .status-badge {
      padding: 3px 8px;
      border-radius: 12px;
      font-size: 0.75em;
      font-weight: bold;
      color: white;
      text-transform: uppercase;
    }
    .status-added { background-color: #4caf50; /* Green */ }
    .status-modified { background-color: #ff9800; /* Orange */ }
    .status-deleted { background-color: #f44336; /* Red */ }
    .status-renamed { background-color: #2196f3; /* Blue */ }
    .status-copied { background-color: #9c27b0; /* Purple */ }
    .status-unknown { background-color: #757575; /* Grey */ }
    .file-name { font-family: "SFMono-Regular", Consolas, "Liberation Mono", Menlo, Courier, monospace; }
    `
  ],
  providers: [
  ]
})
export class GitBrowserComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private apiUrl = '/api';

  repositories: GitRepo[] = [];
  branches: GitBranch[] = [];
  commits: GitCommit[] = [];
  changes: GitCommitChange[] = [];

  selectedRepoName: string | null = null;
  selectedBranchName: string | null = null;
  selectedCommitSha: string | null = null;

  loadingRepos = false;
  loadingBranches = false;
  loadingCommits = false;
  loadingChanges = false;

  private repoSub: Subscription | undefined;
  private branchSub: Subscription | undefined;
  private commitSub: Subscription | undefined;
  private changeSub: Subscription | undefined;

  constructor() { }

  ngOnInit(): void {
    this.fetchRepositories();
  }

  fetchRepositories(): void {
    this.loadingRepos = true;
    this.selectedRepoName = null;
    this.selectedBranchName = null;
    this.selectedCommitSha = null;
    this.repositories = [];
    this.branches = [];
    this.commits = [];
    this.changes = [];
    this.repoSub = this.http.get<GitRepo[]>(`${this.apiUrl}/repos`).subscribe({
      next: data => {
        this.repositories = data;
        this.loadingRepos = false;
      },
      error: () => {
        this.loadingRepos = false;
        console.error('Error fetching repositories');
      }
    });
  }

  onSelectRepo(repoName: string): void {
    if (this.loadingBranches) return;
    if (this.selectedRepoName === repoName && this.branches.length > 0 && !this.branches.find(b => !b.name)) return;

    this.selectedRepoName = repoName;
    this.selectedBranchName = null;
    this.selectedCommitSha = null;
    this.branches = [];
    this.commits = [];
    this.changes = [];

    this.fetchBranches(repoName);
  }

  fetchBranches(repoName: string): void {
    if (!repoName) return;
    this.loadingBranches = true;
    this.branchSub = this.http.get<GitBranch[]>(`${this.apiUrl}/repos/${repoName}/branches`).subscribe({
      next: data => {
        this.branches = data;
        this.loadingBranches = false;
      },
      error: () => {
        this.loadingBranches = false;
        console.error(`Error fetching branches for ${repoName}`);
      }
    });
  }

  onSelectBranch(branchName: string): void {
    if (!this.selectedRepoName || this.loadingCommits) return;
    if (this.selectedBranchName === branchName && this.commits.length > 0 && !this.commits.find(c => !c.sha)) return;

    this.selectedBranchName = branchName;
    this.selectedCommitSha = null;
    this.commits = [];
    this.changes = [];

    this.fetchCommits(this.selectedRepoName, branchName);
  }

  fetchCommits(repoName: string, branchName: string): void {
    if (!repoName || !branchName) return;
    this.loadingCommits = true;
    this.commitSub = this.http.get<GitCommit[]>(`${this.apiUrl}/repos/${repoName}/branches/${branchName}/commits`).subscribe({
      next: data => {
        this.commits = data;
        this.loadingCommits = false;
      },
      error: () => {
        this.loadingCommits = false;
        console.error(`Error fetching commits for ${repoName}/${branchName}`);
      }
    });
  }

  onSelectCommit(commitSha: string): void {
    if (!this.selectedRepoName || !this.selectedBranchName || this.loadingChanges) return;
    if (this.selectedCommitSha === commitSha && this.changes.length > 0 && !this.changes.find(c => !c.fileName)) return;

    this.selectedCommitSha = commitSha;
    this.changes = [];

    this.fetchCommitChanges(this.selectedRepoName, commitSha);
  }

  fetchCommitChanges(repoName: string, commitSha: string): void {
    if (!repoName || !commitSha) return;
    this.loadingChanges = true;
    this.changeSub = this.http.get<GitCommitChange[]>(`${this.apiUrl}/repos/${repoName}/commits/${commitSha}/changes`).subscribe({
      next: data => {
        this.changes = data;
        this.loadingChanges = false;
      },
      error: () => {
        this.loadingChanges = false;
        console.error(`Error fetching changes for commit ${commitSha}`);
      }
    });
  }

  ngOnDestroy(): void {
    this.repoSub?.unsubscribe();
    this.branchSub?.unsubscribe();
    this.commitSub?.unsubscribe();
    this.changeSub?.unsubscribe();
  }
}
