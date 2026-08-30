export type LinkItem = {
  code: string;
  isCustomAlias: boolean;
  shortUrl: string;
  originalUrl: string;
  defaultUrl?: string;
  iosUrl?: string;
  androidUrl?: string;
  clicks: number;
  createdAt: string;
  lastAccessedAt?: string;
  isDisabled: boolean;
};

export type CreateLinkValues = {
  url: string;
  alias?: string;
  defaultUrl?: string;
  iosUrl?: string;
  androidUrl?: string;
};
