import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Repository {
  name: string;
  path: string;
}

export interface Branch {
  name: string;
  isRemote: boolean;
}

export interface Commit {
  sha: string;
  authorName: string;
  authorEmail: string;
  date: string; // Consider using Date type and parsing
  message: string;
}

export interface CommitChange {
  fileName: string;
  status: string; // e.g., "Added", "Modified", "Deleted"
}

@Injectable({
  providedIn: 'root'
})
export class GitService {

  private apiUrl = 'http://localhost:5264/api'; // Base API URL

  constructor(private http: HttpClient) { }

  getRepositories(): Observable<Repository[]> {
    return this.http.get<Repository[]>(`${this.apiUrl}/repos`);
  }

  getBranches(repoName: string): Observable<Branch[]> {
    return this.http.get<Branch[]>(`${this.apiUrl}/repos/${repoName}/branches`);
  }

  getCommits(repoName: string, branchName: string): Observable<Commit[]> {
    // Ensure branchName is URL encoded if it can contain special characters
    return this.http.get<Commit[]>(`${this.apiUrl}/repos/${repoName}/branches/${encodeURIComponent(branchName)}/commits`);
  }

  getCommitChanges(repoName: string, commitSha: string): Observable<CommitChange[]> {
    return this.http.get<CommitChange[]>(`${this.apiUrl}/repos/${repoName}/commits/${commitSha}/changes`);
  }
}
