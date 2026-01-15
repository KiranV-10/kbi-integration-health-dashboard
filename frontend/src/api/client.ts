import type {
  HealthCheckResultDto,
  IncidentCreateRequest,
  IncidentDto,
  IncidentResolveRequest,
  IncidentStatus,
  IncidentStatusUpdateRequest,
  ServiceSummaryDto,
} from "./types";

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL?.toString() ?? "http://localhost:5000";

async function fetchJson<T>(input: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${input}`, {
    headers: {
      "Content-Type": "application/json",
    },
    ...init,
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Request failed with ${response.status}`);
  }

  return (await response.json()) as T;
}

export function getServices(params: {
  activeOnly?: boolean;
  search?: string;
}): Promise<ServiceSummaryDto[]> {
  const query = new URLSearchParams();
  if (params.activeOnly) {
    query.set("activeOnly", "true");
  }
  if (params.search) {
    query.set("search", params.search);
  }
  const qs = query.toString();
  return fetchJson<ServiceSummaryDto[]>(`/api/services${qs ? `?${qs}` : ""}`);
}

export function runChecks(): Promise<HealthCheckResultDto[]> {
  return fetchJson<HealthCheckResultDto[]>("/api/checks/run", {
    method: "POST",
  });
}

export function getIncidents(status?: IncidentStatus): Promise<IncidentDto[]> {
  const query = status ? `?status=${encodeURIComponent(status)}` : "";
  return fetchJson<IncidentDto[]>(`/api/incidents${query}`);
}

export function createIncident(
  request: IncidentCreateRequest,
): Promise<IncidentDto> {
  return fetchJson<IncidentDto>("/api/incidents", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function updateIncidentStatus(
  id: number,
  request: IncidentStatusUpdateRequest,
): Promise<IncidentDto> {
  return fetchJson<IncidentDto>(`/api/incidents/${id}/status`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}

export function resolveIncident(
  id: number,
  request: IncidentResolveRequest,
): Promise<IncidentDto> {
  return fetchJson<IncidentDto>(`/api/incidents/${id}/resolve`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}
