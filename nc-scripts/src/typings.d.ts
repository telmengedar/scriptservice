declare var jQuery: any;

interface Process {
    env: Env
}

interface Env {
    AUTH_URL: string
    CLIENT_ID: string
    CLIENT_SECRET: string
}

interface GlobalEnvironment{
    process: Process;
}

declare var appprocess: Process;