FROM collabora/code:latest
ENV domain localhost
COPY ./coolwsd.xml /etc/coolwsd/coolwsd.xml
COPY ./proof_key /etc/coolwsd/proof_key
COPY ./proof_key.pub /etc/coolwsd/proof_key.pub
COPY ./start-collabora-online.sh /
USER root
RUN chmod +x start-collabora-online.sh
# switch to cool user (use numeric user id to be compatible with Kubernetes Pod Security Policies)
USER 104
# Entry point
CMD ["/start-collabora-online.sh"]