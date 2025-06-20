import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { GitApi, Repository } from '../../services/git-api'; // Adjusted import path

@Component({
  selector: 'app-repositories',
  standalone: true,
  imports: [
    CommonModule,
    MatListModule,
    MatCardModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './repositories.html',
  styleUrl: './repositories.css'
})
export class Repositories implements OnInit {
  repositories$: Observable<Repository[]> | undefined;
  @Output() repoSelected = new EventEmitter<string>();

  constructor(private gitApiService: GitApi) {}

  ngOnInit(): void {
    this.loadRepositories();
  }

  loadRepositories(): void {
    this.repositories$ = this.gitApiService.getRepositories();
  }

  onRepoClick(repoName: string): void {
    this.repoSelected.emit(repoName);
  }
}
