export type HealthStatus = "Ok" | "Degraded" | "Down";
export type IncidentSeverity = "Low" | "Medium" | "High" | "Critical";
export type IncidentStatus = "Open" | "Investigating" | "Mitigated" | "Resolved";

export interface ServiceSummaryDto {
  id: number;
  name: string;
  description?: string | null;
  baseUrl: string;
  ownerTeam?: string | null;
  isActive: boolean;
  lastCheckedAtUtc?: string | null;
  lastStatus?: HealthStatus | null;
  latencyMs?: number | null;
  lastErrorMessage?: string | null;
  openIncidentCount: number;
}

export interface HealthCheckResultDto {
  serviceEndpointId: number;
  checkedAtUtc: string;
  status: HealthStatus;
  latencyMs?: number | null;
  httpStatusCode?: number | null;
  errorMessage?: string | null;
}

export interface IncidentDto {
  id: number;
  serviceEndpointId: number;
  title: string;
  severity: IncidentSeverity;
  status: IncidentStatus;
  reportedBy: string;
  reportedAtUtc: string;
  description?: string | null;
  resolutionNotes?: string | null;
  resolvedAtUtc?: string | null;
}

export interface IncidentCreateRequest {
  serviceEndpointId: number;
  title: string;
  severity: IncidentSeverity;
  reportedBy: string;
  description?: string | null;
}

export interface IncidentStatusUpdateRequest {
  status: IncidentStatus;
  notes?: string | null;
}

export interface IncidentResolveRequest {
  resolutionNotes?: string | null;
}
