import axios from "axios";
import type { CreateLinkValues, LinkItem } from "../types/link";

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? "",
  headers: { "Content-Type": "application/json" },
});

export function getApiErrorMessage(
  error: unknown,
  fallback = "Something went wrong",
) {
  if (axios.isAxiosError<{ message?: string }>(error))
    return error.response?.data?.message ?? fallback;
  return error instanceof Error ? error.message : fallback;
}

export const linkApi = {
  async getAll() {
    const { data } = await apiClient.get<LinkItem[]>("/api/links");
    return data;
  },
  async create(values: CreateLinkValues) {
    const { data } = await apiClient.post<LinkItem>("/api/links", values);
    return data;
  },
  async disable(code: string) {
    await apiClient.patch(`/api/links/${code}/disable`);
  },
  async remove(code: string) {
    await apiClient.delete(`/api/links/${code}`);
  },
};
