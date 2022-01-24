import { setGetFetchOpts, fetchJSON } from '@helpers/fetch';
import { FetchResponse } from '@appTypes/fetch';

export default async function handler(req, res) {

    const apiUrl: string = req.originalUrl.split(`gateway`)[1];
    const apiResponse: FetchResponse = await fetchJSON(process.env.NEXT_PUBLIC_API_BASE_URL + apiUrl, setGetFetchOpts({
        'Authorization': `Basic ${process.env.SHAREDSECRETS_APIAPPLICATION}`
    }), 30000);
    const apiData: any = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { status } = apiMeta;

    res.status(status).json(apiData);

}
