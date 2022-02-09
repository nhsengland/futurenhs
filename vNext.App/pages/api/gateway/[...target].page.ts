import { setGetFetchOpts, setPostFetchOpts, fetchJSON } from '@helpers/fetch';
import { validate } from '@helpers/validators';
import formConfigs from '@formConfigs/index';
import { FetchOptions, FetchResponse } from '@appTypes/fetch';
import { Form } from '@appTypes/form';

export default async function handler(req, res) {

    const apiUrl: string = req.originalUrl.split(`gateway`)[1];
    const method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE' = req.method;
    const headers: any = {
        'Authorization': `Bearer ${process.env.SHAREDSECRETS_APIAPPLICATION}`
    };

    if(method === 'POST'){

        if(!req.body?.formId || !formConfigs[req.body?.formId]){

            return res.status(400).message('Post body missing valid formId');

        }

        const formConfig: Form = formConfigs[req.body.formId];
        const validationErrors = validate(req.body, formConfig.steps[0].fields);

        if(Object.keys(validationErrors).length > 0){

            return res.status(400).json(validationErrors);

        }

    }

    const fetchOpts: FetchOptions = method === 'GET' ? setGetFetchOpts(headers) : setPostFetchOpts(headers, req.body);
    const apiResponse: FetchResponse = await fetchJSON(process.env.NEXT_PUBLIC_API_BASE_URL + apiUrl, fetchOpts, 30000);

    const apiData: any = apiResponse.json;
    const apiMeta: any = apiResponse.meta;

    const { status } = apiMeta;

    return res.status(status).json(apiData);

}
