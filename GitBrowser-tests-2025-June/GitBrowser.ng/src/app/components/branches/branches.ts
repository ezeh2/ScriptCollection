import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { GitApi, Branch } from '../../services/git-api'; // Adjusted import path

@Component({
  selector: 'app-branches',
  standalone: true,
  imports: [
    CommonModule,
    MatListModule,
    MatCardModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './branches.html',
  styleUrl: './branches.css'
})
export class Branches implements OnChanges {
  @Input() repoName: string | null = null;
  @Output() branchSelected = new EventEmitter<string>();
  branches$: Observable<Branch[]> | undefined;

  constructor(private gitApiService: GitApi) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['repoName'] && this.repoName) {
      this.loadBranches(this.repoName);
    } else {
      this.branches$ = undefined; // Clear branches if repoName is null
    }
  }

  loadBranches(repoName: string): void {
    this.branches$ = this.gitApiService.getBranches(repoName);
  }

  onBranchClick(branchName: string): void {
    this.branchSelected.emit(branchName);
  }
}
