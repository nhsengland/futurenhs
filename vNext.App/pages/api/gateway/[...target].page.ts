import { setGetFetchOpts, setPostFetchOpts, fetchJSON } from '@helpers/fetch';
import { validate } from '@helpers/validators';
import { selectFormDefaultFields } from '@selectors/forms';
import formConfigs from '@formConfigs/index';
import { FetchOptions, FetchResponse } from '@appTypes/fetch';

export default async function handler(req, res) {

    const apiUrl: string = req.originalUrl.split(`gateway`)[1];
    const method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE' = req.method;
    const headers: any = {
        'Authorization': `Bearer ${process.env.SHAREDSECRETS_APIAPPLICATION}`
    };

    if(method === 'GET'){

        const fetchOpts: FetchOptions = setGetFetchOpts(headers);
        const apiResponse: FetchResponse = await fetchJSON(process.env.NEXT_PUBLIC_API_BASE_URL + apiUrl, fetchOpts, 30000);
    
        const apiData: any = apiResponse.json;
        const apiMeta: any = apiResponse.meta;
    
        const { status } = apiMeta;
    
        return res.status(status).json(apiData);

    }

    if(method === 'POST'){

        if(!req.body?.['_form-id'] || !formConfigs[req.body?.['_form-id']]){

            console.log('Post body missing valid form-id');

            return res.status(400);

        }

        const validationErrors = validate(req.body, selectFormDefaultFields(formConfigs, req.body['_form-id']), req.body?.['_instance-id']);

        if(Object.keys(validationErrors).length > 0){

            return res.status(400).json(validationErrors);

        }

        // const fetchOpts: FetchOptions = setPostFetchOpts(headers, req.body);
        // const apiResponse: FetchResponse = await fetchJSON(process.env.NEXT_PUBLIC_API_BASE_URL + apiUrl, fetchOpts, 30000);
    
        // const apiData: any = apiResponse.json;
        // const apiMeta: any = apiResponse.meta;
    
        // const { status } = apiMeta;
    
        // return res.status(status).json(apiData);

        return res.status(200).json({});

    }

}
