import { GetServerSideProps } from 'next';

import formConfigs from '@formConfigs/index';
import { selectCsrfToken } from '@selectors/context';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';
import { FormConfig } from '@appTypes/form';

export const withForms = (config: HofConfig, dependencies?: {}): GetServerSideProps => {

    const { props, getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        const csrfToken: string = selectCsrfToken(context);

        const clonedFormConfigs: Record<string, FormConfig> = {};

        /**
         * Ensure the imports are not inadvertently mutated later
         */
        Object.keys(formConfigs).forEach((key) => {

            clonedFormConfigs[key] = {
                id: formConfigs[key].id,
                steps: formConfigs[key].steps
            };

        });

        props.csrfToken = csrfToken;
        props.forms = clonedFormConfigs;

        return await getServerSideProps(context);

    }

}
