import { ScriptLanguage } from './scripts/scriptlanguage';

/**
 * code to execute directly on script server
 */
export interface ScopedCode {

    /**
     * code to execute
     */
    code: string,

    /**
     * name under which to run code
     */
    name: string

    /**
     * language script code is written in
     */
    language: ScriptLanguage
}