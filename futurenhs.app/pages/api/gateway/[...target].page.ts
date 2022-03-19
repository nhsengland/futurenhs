import { setFetchOpts, fetchJSON } from '@helpers/fetch';
import { FetchOptions, FetchResponse } from '@appTypes/fetch';
import { requestMethods } from '@constants/fetch';

export default async function handler(req, res) {

    const apiUrl: string = req.originalUrl.split(`gateway`)[1];
    const method: requestMethods = req.method;
    const headers: any = {
        'Authorization': `Bearer ${process.env.SHAREDSECRETS_APIAPPLICATION}`
    };

    const fetchOpts: FetchOptions = setFetchOpts({ method, headers, body: req.body });
    const apiResponse: FetchResponse = await fetchJSON(process.env.NEXT_PUBLIC_API_BASE_URL + apiUrl, fetchOpts, 30000);

    const apiData: any = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { status } = apiMeta;

    return res.status(status).json(apiData);

}
