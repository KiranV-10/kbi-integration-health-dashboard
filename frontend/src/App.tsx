import { type FormEvent, useEffect, useMemo, useState } from "react";
import "./App.css";
import {
  createIncident,
  getIncidents,
  getServices,
  resolveIncident,
  runChecks,
  updateIncidentStatus,
} from "./api/client";
import type {
  IncidentCreateRequest,
  IncidentDto,
  IncidentSeverity,
  IncidentStatus,
  ServiceSummaryDto,
} from "./api/types";

const severityOptions: IncidentSeverity[] = [
  "Low",
  "Medium",
  "High",
  "Critical",
];
const statusOptions: IncidentStatus[] = [
  "Open",
  "Investigating",
  "Mitigated",
  "Resolved",
];

function formatDate(value?: string | null) {
  if (!value) {
    return "-";
  }
  return new Date(value).toLocaleString();
}

function App() {
  const [services, setServices] = useState<ServiceSummaryDto[]>([]);
  const [servicesLoading, setServicesLoading] = useState(false);
  const [servicesError, setServicesError] = useState<string | null>(null);
  const [activeOnly, setActiveOnly] = useState(true);
  const [search, setSearch] = useState("");
  const [checksRunning, setChecksRunning] = useState(false);

  const [incidents, setIncidents] = useState<IncidentDto[]>([]);
  const [incidentsLoading, setIncidentsLoading] = useState(false);
  const [incidentsError, setIncidentsError] = useState<string | null>(null);
  const [incidentStatusFilter, setIncidentStatusFilter] = useState<
    IncidentStatus | "all"
  >("Open");
  const [selectedIncidentId, setSelectedIncidentId] = useState<number | null>(
    null,
  );

  const [incidentForm, setIncidentForm] = useState<IncidentCreateRequest>({
    serviceEndpointId: 0,
    title: "",
    severity: "Medium",
    reportedBy: "",
    description: "",
  });
  const [incidentFormError, setIncidentFormError] = useState<string | null>(
    null,
  );
  const [incidentFormLoading, setIncidentFormLoading] = useState(false);

  const [statusUpdate, setStatusUpdate] = useState({
    status: "Open" as IncidentStatus,
    notes: "",
  });
  const [resolveNotes, setResolveNotes] = useState("");
  const [detailError, setDetailError] = useState<string | null>(null);

  const selectedIncident = useMemo(
    () => incidents.find((incident) => incident.id === selectedIncidentId),
    [incidents, selectedIncidentId],
  );

  const loadServices = async () => {
    setServicesLoading(true);
    setServicesError(null);
    try {
      const data = await getServices({
        activeOnly,
        search: search.trim() ? search : undefined,
      });
      setServices(data);
    } catch (error) {
      setServicesError((error as Error).message);
    } finally {
      setServicesLoading(false);
    }
  };

  const loadIncidents = async () => {
    setIncidentsLoading(true);
    setIncidentsError(null);
    try {
      const status =
        incidentStatusFilter === "all" ? undefined : incidentStatusFilter;
      const data = await getIncidents(status);
      setIncidents(data);
      if (data.length > 0 && selectedIncidentId === null) {
        setSelectedIncidentId(data[0].id);
      }
    } catch (error) {
      setIncidentsError((error as Error).message);
    } finally {
      setIncidentsLoading(false);
    }
  };

  useEffect(() => {
    void loadServices();
  }, [activeOnly, search]);

  useEffect(() => {
    void loadIncidents();
  }, [incidentStatusFilter]);

  useEffect(() => {
    if (services.length > 0 && incidentForm.serviceEndpointId === 0) {
      setIncidentForm((prev) => ({
        ...prev,
        serviceEndpointId: services[0].id,
      }));
    }
  }, [services, incidentForm.serviceEndpointId]);

  useEffect(() => {
    if (selectedIncident) {
      setStatusUpdate({
        status: selectedIncident.status,
        notes: selectedIncident.resolutionNotes ?? "",
      });
      setResolveNotes(selectedIncident.resolutionNotes ?? "");
    }
  }, [selectedIncident]);

  const handleRunChecks = async () => {
    setChecksRunning(true);
    try {
      await runChecks();
      await loadServices();
    } catch (error) {
      setServicesError((error as Error).message);
    } finally {
      setChecksRunning(false);
    }
  };

  const handleCreateIncident = async (event: FormEvent) => {
    event.preventDefault();
    setIncidentFormError(null);

    if (!incidentForm.title.trim() || !incidentForm.reportedBy.trim()) {
      setIncidentFormError("Title and reported by are required.");
      return;
    }

    setIncidentFormLoading(true);
    try {
      await createIncident({
        ...incidentForm,
        title: incidentForm.title.trim(),
        reportedBy: incidentForm.reportedBy.trim(),
        description: incidentForm.description?.trim() || undefined,
      });
      setIncidentForm({
        serviceEndpointId: incidentForm.serviceEndpointId,
        title: "",
        severity: incidentForm.severity,
        reportedBy: "",
        description: "",
      });
      await Promise.all([loadServices(), loadIncidents()]);
    } catch (error) {
      setIncidentFormError((error as Error).message);
    } finally {
      setIncidentFormLoading(false);
    }
  };

  const handleStatusUpdate = async (event: FormEvent) => {
    event.preventDefault();
    if (!selectedIncident) {
      return;
    }
    setDetailError(null);
    try {
      await updateIncidentStatus(selectedIncident.id, {
        status: statusUpdate.status,
        notes: statusUpdate.notes?.trim() || undefined,
      });
      await loadIncidents();
    } catch (error) {
      setDetailError((error as Error).message);
    }
  };

  const handleResolve = async () => {
    if (!selectedIncident) {
      return;
    }
    setDetailError(null);
    try {
      await resolveIncident(selectedIncident.id, {
        resolutionNotes: resolveNotes.trim() || undefined,
      });
      await loadIncidents();
    } catch (error) {
      setDetailError((error as Error).message);
    }
  };

  return (
    <div className="page">
      <header className="page-header">
        <div>
          <p className="eyebrow">Integration Operations</p>
          <h1>Integration Health & Incident Dashboard</h1>
          <p className="subtitle">
            Monitor integration health, run checks, and manage incidents.
          </p>
        </div>
        <button
          className="primary"
          onClick={handleRunChecks}
          disabled={checksRunning}
        >
          {checksRunning ? "Running checks..." : "Run Checks Now"}
        </button>
      </header>

      <section className="panel">
        <div className="panel-header">
          <h2>Service Health</h2>
          <div className="filters">
            <label className="checkbox">
              <input
                type="checkbox"
                checked={activeOnly}
                onChange={(event) => setActiveOnly(event.target.checked)}
              />
              Active only
            </label>
            <input
              type="search"
              placeholder="Search by name"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
          </div>
        </div>
        {servicesLoading ? (
          <p className="state">Loading services...</p>
        ) : servicesError ? (
          <p className="state error">{servicesError}</p>
        ) : services.length === 0 ? (
          <p className="state">No services match the current filters.</p>
        ) : (
          <div className="table">
            <div className="table-row table-header">
              <span>Name</span>
              <span>Status</span>
              <span>Last Checked</span>
              <span>Latency</span>
              <span>Last Error</span>
              <span>Open Incidents</span>
            </div>
            {services.map((service) => (
              <div key={service.id} className="table-row">
                <div>
                  <strong>{service.name}</strong>
                  <p className="muted">{service.ownerTeam ?? "-"}</p>
                </div>
                <span className={`badge ${service.lastStatus ?? "unknown"}`}>
                  {service.lastStatus ?? "Unknown"}
                </span>
                <span>{formatDate(service.lastCheckedAtUtc)}</span>
                <span>
                  {service.latencyMs !== null && service.latencyMs !== undefined
                    ? `${service.latencyMs} ms`
                    : "-"}
                </span>
                <span className="muted">
                  {service.lastErrorMessage ?? "-"}
                </span>
                <span>{service.openIncidentCount}</span>
              </div>
            ))}
          </div>
        )}
      </section>

      <section className="panel">
        <div className="panel-header">
          <h2>Incidents</h2>
          <div className="filters">
            <label>
              Status
              <select
                value={incidentStatusFilter}
                onChange={(event) =>
                  setIncidentStatusFilter(
                    event.target.value as IncidentStatus | "all",
                  )
                }
              >
                <option value="all">All</option>
                {statusOptions.map((status) => (
                  <option key={status} value={status}>
                    {status}
                  </option>
                ))}
              </select>
            </label>
          </div>
        </div>

        <div className="incident-grid">
          <div className="incident-list">
            {incidentsLoading ? (
              <p className="state">Loading incidents...</p>
            ) : incidentsError ? (
              <p className="state error">{incidentsError}</p>
            ) : incidents.length === 0 ? (
              <p className="state">No incidents for the selected filter.</p>
            ) : (
              incidents.map((incident) => (
                <button
                  key={incident.id}
                  className={`incident-card ${
                    incident.id === selectedIncidentId ? "active" : ""
                  }`}
                  onClick={() => setSelectedIncidentId(incident.id)}
                >
                  <div>
                    <strong>{incident.title}</strong>
                    <p className="muted">
                      {incident.severity} • {incident.status}
                    </p>
                  </div>
                  <span className="muted">{formatDate(incident.reportedAtUtc)}</span>
                </button>
              ))
            )}
          </div>

          <div className="incident-detail">
            <div className="detail-header">
              <h3>Incident Detail</h3>
            </div>
            {selectedIncident ? (
              <>
                <div className="detail-body">
                  <p className="muted">Service ID: {selectedIncident.serviceEndpointId}</p>
                  <h4>{selectedIncident.title}</h4>
                  <p>{selectedIncident.description ?? "No description provided."}</p>
                  <div className="detail-meta">
                    <span>Severity: {selectedIncident.severity}</span>
                    <span>Status: {selectedIncident.status}</span>
                    <span>Reported by: {selectedIncident.reportedBy}</span>
                    <span>Reported: {formatDate(selectedIncident.reportedAtUtc)}</span>
                  </div>
                  {selectedIncident.resolutionNotes && (
                    <div className="detail-notes">
                      <strong>Resolution Notes</strong>
                      <p>{selectedIncident.resolutionNotes}</p>
                    </div>
                  )}
                  {selectedIncident.resolvedAtUtc && (
                    <p className="muted">
                      Resolved: {formatDate(selectedIncident.resolvedAtUtc)}
                    </p>
                  )}
                </div>

                <form className="detail-form" onSubmit={handleStatusUpdate}>
                  <h4>Update Status</h4>
                  <label>
                    Status
                    <select
                      value={statusUpdate.status}
                      onChange={(event) =>
                        setStatusUpdate((prev) => ({
                          ...prev,
                          status: event.target.value as IncidentStatus,
                        }))
                      }
                    >
                      {statusOptions.map((status) => (
                        <option key={status} value={status}>
                          {status}
                        </option>
                      ))}
                    </select>
                  </label>
                  <label>
                    Notes
                    <textarea
                      rows={3}
                      value={statusUpdate.notes}
                      onChange={(event) =>
                        setStatusUpdate((prev) => ({
                          ...prev,
                          notes: event.target.value,
                        }))
                      }
                    />
                  </label>
                  <button type="submit" className="primary">
                    Save Status
                  </button>
                </form>

                <div className="detail-form">
                  <h4>Resolve Incident</h4>
                  <label>
                    Resolution Notes
                    <textarea
                      rows={3}
                      value={resolveNotes}
                      onChange={(event) => setResolveNotes(event.target.value)}
                    />
                  </label>
                  <button type="button" onClick={handleResolve}>
                    Mark Resolved
                  </button>
                </div>

                {detailError && <p className="state error">{detailError}</p>}
              </>
            ) : (
              <p className="state">Select an incident to view details.</p>
            )}
          </div>

          <form className="incident-form" onSubmit={handleCreateIncident}>
            <h3>Create Incident</h3>
            <label>
              Service
              <select
                value={incidentForm.serviceEndpointId}
                onChange={(event) =>
                  setIncidentForm((prev) => ({
                    ...prev,
                    serviceEndpointId: Number(event.target.value),
                  }))
                }
              >
                {services.map((service) => (
                  <option key={service.id} value={service.id}>
                    {service.name}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Title
              <input
                type="text"
                value={incidentForm.title}
                onChange={(event) =>
                  setIncidentForm((prev) => ({
                    ...prev,
                    title: event.target.value,
                  }))
                }
              />
            </label>
            <label>
              Severity
              <select
                value={incidentForm.severity}
                onChange={(event) =>
                  setIncidentForm((prev) => ({
                    ...prev,
                    severity: event.target.value as IncidentSeverity,
                  }))
                }
              >
                {severityOptions.map((severity) => (
                  <option key={severity} value={severity}>
                    {severity}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Reported By
              <input
                type="text"
                value={incidentForm.reportedBy}
                onChange={(event) =>
                  setIncidentForm((prev) => ({
                    ...prev,
                    reportedBy: event.target.value,
                  }))
                }
              />
            </label>
            <label>
              Description
              <textarea
                rows={4}
                value={incidentForm.description ?? ""}
                onChange={(event) =>
                  setIncidentForm((prev) => ({
                    ...prev,
                    description: event.target.value,
                  }))
                }
              />
            </label>
            <button type="submit" className="primary" disabled={incidentFormLoading}>
              {incidentFormLoading ? "Creating..." : "Create Incident"}
            </button>
            {incidentFormError && (
              <p className="state error">{incidentFormError}</p>
            )}
          </form>
        </div>
      </section>
    </div>
  );
}

export default App;
