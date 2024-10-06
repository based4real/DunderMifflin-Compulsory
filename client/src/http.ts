import {Api} from './Api.ts';
const baseUrl = import.meta.env.VITE_APP_BASE_API_URL

export const api = new Api({
    baseURL: baseUrl,
    headers: {
        "Prefer": "return=representation"
    }
});