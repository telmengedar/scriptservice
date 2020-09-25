import { ScriptLanguage } from './scriptlanguage';

/**
 * scriptcode which can get executed on server
 */
export interface Script {

    /**
     * script id
     */
    id?: number,

    /**
     * script revision
     */
    revision?: number,

    /**
     * name of script
     */
    name: string,

    /**
     * script code
     */
    code: string,

    /**
     * language script code is written in
     */
    language: ScriptLanguage
}