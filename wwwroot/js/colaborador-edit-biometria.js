document.addEventListener("DOMContentLoaded", function () {

    const btnCadastrarBiometria = document.getElementById("btnCadastrarBiometria");
    const statusBiometria = document.getElementById("statusBiometria");
    const colaboradorIdHidden = document.getElementById("colaboradorIdHidden"); // Input hidden com o ID

    if (!btnCadastrarBiometria || !colaboradorIdHidden) {
        console.warn("Botão de biometria ou ID do colaborador não encontrado nesta página.");
        return; // Sai se os elementos não estiverem presentes
    }

    btnCadastrarBiometria.addEventListener("click", async function () {

        const colaboradorId = parseInt(colaboradorIdHidden.value, 10);

        // 1. Validação Simples (ID deve existir)
        if (!colaboradorId || colaboradorId === 0) {
            exibirMensagem("ID do colaborador não encontrado na página.", "danger");
            return;
        }

        btnCadastrarBiometria.disabled = true;
        exibirMensagem("Iniciando captura...", "info");

        try {
            // 2. Chamar o leitor local
            const leitorUrl = "http://localhost:8000/capture"; // Endpoint do software local
            exibirMensagem("Aguardando leitor... Posicione o dedo.", "info");

            const responseLeitor = await fetch(leitorUrl, { method: "GET", mode: "cors" });

            if (!responseLeitor.ok) {
                throw new Error(`Falha ao conectar no leitor. (Status: ${responseLeitor.status})`);
            }

            const dataLeitor = await responseLeitor.json();

            if (!dataLeitor.success || !dataLeitor.template) {
                throw new Error(dataLeitor.message || "Leitor não retornou um template válido.");
            }

            const templateBase64 = dataLeitor.template;
            exibirMensagem("Digital capturada! Salvando no sistema...", "info");

            // 3. Enviar para o servidor do Portal (Novo Handler no Edit.cshtml.cs)
            const requestBody = {
                ColaboradorId: colaboradorId,
                BiometriaTemplateBase64: templateBase64
            };

            const antiforgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;

            // O handler agora está na própria página de edição
            const responseServidor = await fetch("?handler=CadastrarBiometria", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": antiforgeryToken
                },
                body: JSON.stringify(requestBody)
            });

            const dataServidor = await responseServidor.json();

            // 4. Exibir resultado final
            if (responseServidor.ok && dataServidor.success) {
                exibirMensagem(dataServidor.message || "Biometria cadastrada com sucesso!", "success");
            } else {
                // Usa a mensagem do servidor se disponível, senão uma genérica
                throw new Error(dataServidor.message || `Erro do servidor (Status: ${responseServidor.status})`);
            }

        } catch (error) {
            console.error("Erro no cadastro de biometria:", error);
            let mensagemErro = error.message;

            if (error.message.includes("Failed to fetch")) {
                mensagemErro = "<strong>Falha de conexão.</strong><br>Não foi possível conectar ao software do leitor em <code>http://localhost:9000</code>. Verifique se ele está em execução.";
            } else if (error.message.includes("Status: 400")) {
                mensagemErro = "Erro nos dados enviados ou ID inconsistente. Verifique se o colaborador está correto.";
            } else if (error.message.includes("Status: 500")) {
                mensagemErro = "Erro interno no servidor ao salvar a biometria. Tente novamente ou contate o suporte.";
            }


            exibirMensagem(mensagemErro, "danger");
        } finally {
            btnCadastrarBiometria.disabled = false;
        }
    });

    function exibirMensagem(mensagem, tipo) {
        statusBiometria.innerHTML = `<div class="alert alert-${tipo}" role="alert">${mensagem}</div>`;
    }

});