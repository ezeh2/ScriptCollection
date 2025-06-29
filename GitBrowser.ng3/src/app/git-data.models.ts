export interface GitRepo {
  name: string;
  path: string; // Even if same as name, it's in the API response
}

export interface GitBranch {
  name: string;
  isRemote: boolean;
}

export interface GitCommit {
  sha: string;
  message: string;
  messageShort: string;
  author: string;
  authorDate: string; // Representing DateTime as string for simplicity in TS
  committer: string;
  committerDate: string; // Representing DateTime as string
}

export interface GitCommitChange {
  fileName: string;
  status: string;
}
