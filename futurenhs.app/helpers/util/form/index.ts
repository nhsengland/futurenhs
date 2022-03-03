import { FormData } from 'formdata-node';

/**
 * Returns relevant aria attributes for a field's current state
 */
export const getServerSideMultiPartFormData = (body: Record<any, any>): FormData => {

    const formData: any = new FormData();

    for(const fieldName in body){

        formData.set(fieldName, body[fieldName]);

    }

    return formData;

};

/**
 * Returns relevant aria attributes for a field's current state
 */
export const getAriaFieldAttributes = (isRequired: boolean, isError: boolean, describedBy?: Array<string>, labelledBy?: string): any => {

    let ariaProps: any = {};

    if (isRequired) {

        ariaProps['aria-required'] = 'true';

    }

    if (isError) {

        ariaProps['aria-invalid'] = 'true';

    }

    if (describedBy?.length) {

        let ids: string = '';

        describedBy.forEach((id: string) => {

            if (id) {

                ids += `${ids ? ' ' : ''}${id}`

            }

        });

        if (ids) {

            ariaProps['aria-describedby'] = ids;

        }

    }

    if (labelledBy) {

        ariaProps['aria-labelledby'] = labelledBy;

    }

    return ariaProps;

};