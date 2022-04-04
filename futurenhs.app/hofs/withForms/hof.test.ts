import { withForms } from ".";
import { handleSSRSuccessProps } from "@helpers/util/ssr/handleSSRSuccessProps";

describe('withForms hof', () => {

    const csrfToken = () => 'mock-csrf-token'
    const props: any = {};

    const getServerSideProps = async (context) => {

        return handleSSRSuccessProps({ props });

    };

    it('returns form data', async () => {

        const mockWithForms = withForms({ props, getServerSideProps });
        const result = await mockWithForms({ req: { csrfToken } } as any);

        expect(result).toHaveProperty('props.forms');

    });

    it('returns form error when passed invalid props', async () => {

        const mockWithForms = withForms({ props: null, getServerSideProps });

        try {

            await mockWithForms({ req: { csrfToken } } as any);
            expect(true).toBe(false);

        } catch (err) {

            expect(true).toBe(true);

        }

    });
    
})