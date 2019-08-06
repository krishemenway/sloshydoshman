CREATE TABLE public.server
(
    server_id uuid NOT NULL,
    name text COLLATE pg_catalog."default",
    last_known_ip text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT px_server_id PRIMARY KEY (server_id)
) WITH (OIDS = FALSE) TABLESPACE pg_default;

ALTER TABLE public.server OWNER to postgres;
GRANT ALL ON TABLE public.server TO postgres;
GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.server TO kf2statsuser;