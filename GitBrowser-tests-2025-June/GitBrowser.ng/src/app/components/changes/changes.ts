import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { GitApi, Change } from '../../services/git-api'; // Adjusted import path

@Component({
  selector: 'app-changes',
  standalone: true,
  imports: [
    CommonModule,
    MatListModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatExpansionModule
  ],
  templateUrl: './changes.html',
  styleUrl: './changes.css'
})
export class Changes implements OnChanges {
  @Input() repoName: string | null = null;
  @Input() commitSha: string | null = null; // Corrected typo from @Inputn to @Input
  changes$: Observable<Change[]> | undefined;

  constructor(private gitApiService: GitApi) {}

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['repoName'] || changes['commitSha']) && this.repoName && this.commitSha) {
      this.loadChanges(this.repoName, this.commitSha);
    } else {
      this.changes$ = undefined; // Clear changes if inputs are not set
    }
  }

  loadChanges(repoName: string, commitSha: string): void {
    this.changes$ = this.gitApiService.getChanges(repoName, commitSha);
  }
}
