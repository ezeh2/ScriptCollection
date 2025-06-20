import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Data Models
export interface Repository {
  name: string;
  description: string;
  html_url: string;
}

export interface Branch {
  name: string;
  commit: {
    sha: string;
    url: string;
  };
}

export interface Commit {
  sha: string;
  commit: {
    author: {
      name: string;
      email: string;
      date: string;
    };
    committer: {
      name: string;
      email: string;
      date: string;
    };
    message: string;
  };
  html_url: string;
  parents: {
    sha: string;
    url: string;
    html_url: string;
  }[];
}

export interface Change {
  filename: string;
  status: string;
  additions: number;
  deletions: number;
  changes: number;
  patch: string;
}

@Injectable({
  providedIn: 'root'
})
export class GitApi {
  private apiUrl = 'http://localhost:5264/api'; // Using a relative path for API calls

  constructor(private http: HttpClient) { }

  getRepositories(): Observable<Repository[]> {
    return this.http.get<Repository[]>(`${this.apiUrl}/repos`);
  }

  getBranches(repoName: string): Observable<Branch[]> {
    return this.http.get<Branch[]>(`${this.apiUrl}/repos/${repoName}/branches`);
  }

  getCommits(repoName: string, branchName: string): Observable<Commit[]> {
    return this.http.get<Commit[]>(`${this.apiUrl}/repos/${repoName}/branches/${branchName}/commits`);
  }

  getChanges(repoName: string, commitSha: string): Observable<Change[]> {
    return this.http.get<Change[]>(`${this.apiUrl}/repos/${repoName}/commits/${commitSha}/changes`);
  }
}
